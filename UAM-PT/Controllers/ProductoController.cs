using DAL;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;
using UAM_PT.Models;

namespace UAM_PT.Controllers
{
    public class ProductoController : Controller
    {
        private readonly NovaMerEntities2 _context = new NovaMerEntities2();

        public IActionResult VerProducto(Guid id)
        {
            var producto = _context.Productoes.FirstOrDefault(p => p.ID == id);

            if (producto == null)
            {
                return NotFound();
            }

            var relacionados = _context.Productoes
                .Where(p => p.CategoriaID == producto.CategoriaID && p.ID != id)
                .Take(24)
                .ToList();

            var model = new ProductoModel
            {
                ID = producto.ID,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.Precio,
                Imagen = producto.Imagen, // si tienes este campo
                Relacionados = relacionados
            };

            return View(model);
        }
    }
}
