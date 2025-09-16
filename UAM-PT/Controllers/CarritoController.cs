using DAL;
using Microsoft.AspNetCore.Mvc;
using UAM_PT.Models;
using System.Data.Entity;

namespace UAM_PT.Controllers
{
    public class CarritoController : Controller
    {
        private readonly NovaMerEntities2 _context = new NovaMerEntities2();

        public IActionResult CarritoDeCompras()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
                return RedirectToAction("Login", "Account");

            var usuarioId = Guid.Parse(userIdString);

            var carrito = _context.Carritoes
                .FirstOrDefault(c => c.UsuarioId == usuarioId && c.Activo);

            if (carrito == null)
            {
                return View(new CarritoModel());
            }

            var productos = _context.CarritoDetalles
                .Where(d => d.CarritoId == carrito.Id)
                .Select(d => new CarritoProductoModel
                {
                    ProductoId = d.ProductoId,
                    Nombre = d.Producto.Nombre,
                    PrecioUnitario = d.PrecioUnitario,
                    Cantidad = d.Cantidad
                })
                .ToList();

            var vm = new CarritoModel { Productos = productos };

            return View(vm);
        }

        public IActionResult Checkout()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
                return RedirectToAction("Login", "Cuenta");

            Guid usuarioId = Guid.Parse(userIdString);

            var carrito = _context.Carritoes
                .Include("CarritoDetalles.Producto")
                .FirstOrDefault(c => c.UsuarioId == usuarioId && c.Activo);



            if (carrito == null || !carrito.CarritoDetalles.Any())
                return RedirectToAction("CarritoDeCompras");

            var model = new CheckoutModel
            {
                Productos = carrito.CarritoDetalles.Select(cd => new CarritoProductoModel
                {
                    ProductoId = cd.ProductoId,
                    Nombre = cd.Producto.Nombre,
                    Cantidad = cd.Cantidad,
                    PrecioUnitario = cd.Producto.Precio
                }).ToList()
            };

            return View(model);
        }

        public IActionResult Agregar(Guid productoId, int cantidad = 1)
        {
            var userIdString = HttpContext.Session.GetString("UserId");

            Guid usuarioId = Guid.Parse(userIdString);

            // Buscar carrito activo
            var carrito = _context.Carritoes.FirstOrDefault(c => c.UsuarioId == usuarioId && c.Activo);

            if (carrito == null)
            {
                carrito = new Carrito
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = usuarioId,
                    FechaCreacion = DateTime.Now,
                    Activo = true
                };
                _context.Carritoes.Add(carrito);
            }

            var detalle = _context.CarritoDetalles
                .FirstOrDefault(d => d.CarritoId == carrito.Id && d.ProductoId == productoId);

            if (detalle != null)
            {
                detalle.Cantidad += cantidad;
            }
            else
            {
                var producto = _context.Productoes.FirstOrDefault(p => p.ID == productoId);
                if (producto == null) return NotFound();

                detalle = new CarritoDetalle
                {
                    Id = Guid.NewGuid(),
                    CarritoId = carrito.Id,
                    ProductoId = producto.ID,
                    Cantidad = cantidad,
                    PrecioUnitario = producto.Precio // campo en tu tabla Producto
                };

                _context.CarritoDetalles.Add(detalle);
            }

            _context.SaveChanges();

            return Json(new { success = true, message = "Producto agregado al carrito" });
        }

        public IActionResult Quitar(Guid productoId)
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
                return Json(new { success = false, message = "Usuario no logueado" });

            Guid usuarioId = Guid.Parse(userIdString);

            // Obtener carrito activo
            var carrito = _context.Carritoes
                .FirstOrDefault(c => c.UsuarioId == usuarioId && c.Activo);

            if (carrito == null)
                return Json(new { success = false, message = "Carrito no encontrado" });

            // Buscar siempre en el mismo contexto
            var detalle = _context.CarritoDetalles
                .FirstOrDefault(d => d.CarritoId == carrito.Id && d.ProductoId == productoId);

            if (detalle != null)
            {
                _context.CarritoDetalles.Remove(detalle);

                try
                {
                    _context.SaveChanges();
                    return Json(new { success = true, message = "Producto quitado" });
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException)
                {
                    // Si llega aquí, el registro ya no existe al momento de SaveChanges
                    return Json(new { success = false, message = "El producto ya fue eliminado del carrito" });
                }
            }

            return Json(new { success = false, message = "Producto no encontrado en el carrito" });
        }

        [HttpPost]
        public IActionResult FinalizarCompra(Guid direccionId)
        {
            try
            {
                var usuarioId = Guid.Parse(HttpContext.Session.GetString("UserId"));

                // 1. Traer carrito activo
                var carrito = _context.Carritoes
                    .Include(c => c.CarritoDetalles.Select(cd => cd.Producto))
                    .FirstOrDefault(c => c.UsuarioId == usuarioId && c.Activo);



                if (carrito == null || !carrito.CarritoDetalles.Any())
                {
                    return Json(new { success = false, message = "El carrito está vacío." });
                }

                // 2. Crear pedido
                var pedido = new Pedido
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = usuarioId,
                    DireccionId = direccionId,
                    FechaPedido = DateTime.Now,
                    Total = carrito.CarritoDetalles.Sum(cd => cd.Cantidad * cd.PrecioUnitario),
                    Estatus = "Pendiente",
                    Activo = true
                };

                _context.Pedidos.Add(pedido);

                // 3. Crear detalles del pedido
                foreach (var item in carrito.CarritoDetalles)
                {
                    var detalle = new DetallesPedido
                    {
                        Id = Guid.NewGuid(),
                        PedidoId = pedido.Id,
                        ProductoId = item.ProductoId,
                        Cantidad = item.Cantidad,
                        Precio = item.PrecioUnitario,
                        Estatus = "Pendiente"
                    };

                    _context.DetallesPedidoes.Add(detalle);
                }

                // 4. Marcar carrito como inactivo
                carrito.Activo = false;

                _context.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


    }
}
