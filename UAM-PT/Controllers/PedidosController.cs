using DAL;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;
using UAM_PT.Models;

namespace UAM_PT.Controllers
{
    public class PedidosController : Controller
    {
        private readonly NovaMerEntities2 _context = new NovaMerEntities2();
        public IActionResult MisPedidos()
        {
            var usuarioId = Guid.Parse(HttpContext.Session.GetString("UserId"));

            var pedidos = _context.Pedidos
                .Where(p => p.UsuarioId == usuarioId && p.Activo)
                .OrderByDescending(p => p.FechaPedido)
                .Select(p => new PedidoModel
                {
                    PedidoId = p.Id,
                    FechaPedido = p.FechaPedido,
                    Estatus = p.Estatus,
                    TotalProductos = p.DetallesPedidoes.Sum(d => d.Cantidad),
                    Productos = p.DetallesPedidoes.Select(d => new ProductoPedidoModel
                    {
                        ProductoId = d.ProductoId,
                        Nombre = d.Producto.Nombre,
                        Precio = d.Precio,
                        Cantidad = d.Cantidad
                    }).ToList()
                }).ToList();

            return View(pedidos);
        }

        public IActionResult DetallesPedido(Guid pedidoId)
        {
            var usuarioId = Guid.Parse(HttpContext.Session.GetString("UserId"));

            var pedido = _context.Pedidos
                .Where(p => p.Id == pedidoId && p.UsuarioId == usuarioId)
                .Select(p => new PedidoModel
                {
                    PedidoId = p.Id,
                    FechaPedido = p.FechaPedido,
                    Estatus = p.Estatus,
                    TotalProductos = p.DetallesPedidoes.Sum(d => d.Cantidad),
                    Productos = p.DetallesPedidoes.Select(d => new ProductoPedidoModel
                    {
                        ProductoId = d.ProductoId,
                        Nombre = d.Producto.Nombre,
                        Precio = d.Precio,
                        Cantidad = d.Cantidad
                    }).ToList()
                }).FirstOrDefault();

            if (pedido == null)
            {
                return RedirectToAction("MisPedidos");
            }

            return View(pedido);
        }
    }
}
