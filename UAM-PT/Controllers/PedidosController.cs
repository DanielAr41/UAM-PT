using Microsoft.AspNetCore.Mvc;

namespace UAM_PT.Controllers
{
    public class PedidosController : Controller
    {
        public IActionResult MisPedidos()
        {
            return View();
        }
    }
}
