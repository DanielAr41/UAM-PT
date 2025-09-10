using DAL;
using Microsoft.AspNetCore.Mvc;
using UAM_PT.Models;

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
            return View();
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





    }
}
