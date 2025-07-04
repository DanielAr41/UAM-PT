using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Diagnostics;
using UAM_PT.Models;

namespace UAM_PT.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Inicio()
        {
            return View();
        }

        public IActionResult Vender()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ObtenProductos()
        {
            using (var ctx = new NovaMerEntities2())
            {
                var productos = ctx.Productoes
                    .Where(x => x.Activo == true)
                    .Select(x => new
                    {
                        x.ID,
                        x.Nombre,
                        x.Precio,
                        x.SKU,
                        x.Stock,
                        x.Peso,
                        x.Descripcion,
                        x.FechaRegistro,
                        x.VendedorID,
                        x.CategoriaID,
                        ImagenUrl = "/Home/ObtenerImagen?id=" + x.ID
                    })
                    .ToList();

                return Json(productos);
            }
        }

        public ActionResult ObtenerImagen(Guid id)
        {
            //using (var db = new NovaMerEntities2())
            //{
            //    var producto = db.Productoes.FirstOrDefault(p => p.ID == id);
            //    if (producto != null && producto.Imagen != null)
            //    {
            //        return File(producto.Imagen, "image/jpeg"); 
            //    }
            //}
            using (var db = new NovaMerEntities2())
            {
                // Obtener la primera imagen activa del producto
                var imagenProducto = db.ImgProductoes
                    .Where(img => img.ProductoID == id && img.Activo == true)
                    .OrderBy(img => img.FechaRegistro)
                    .FirstOrDefault();

                if (imagenProducto != null && imagenProducto.Imagen != null)
                {
                    return File(imagenProducto.Imagen, "image/jpeg");
                }
            }

            return File("~/images/default.jpg", "image/jpg"); 
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
