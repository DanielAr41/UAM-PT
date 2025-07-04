using Microsoft.AspNetCore.Mvc;

namespace UAM_PT.Controllers
{
    public class CarritoController : Controller
    {
        public IActionResult CarritoDeCompras()
        {
            return View();
        }

        public IActionResult Checkout()
        {
            return View();
        }
    }
}
