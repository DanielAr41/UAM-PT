using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Data.Entity;
using UAM_PT.Services;

namespace UAM_PT.Controllers
{
    public class AccountController : Controller
    {
        private readonly EmailService _emailService;
        private readonly NovaMerEntities2 _context = new NovaMerEntities2();

        public AccountController(EmailService emailService)
        {
            _emailService = emailService;
        }

        // Acción para mostrar el formulario de recuperación de contraseña
        public IActionResult RecuperarContrasenia(string email)
        {
            var usuario = _context.Usuarios.FirstOrDefault(x => x.Correo == email);

            if (usuario == null)
            {
                ViewBag.ErrorMessage = "El correo no está registrado";
                return View();
            }

            string codigo = new Random().Next(100000, 999999).ToString();

            HttpContext.Session.SetString("CodigoRecuperacion", codigo);
            HttpContext.Session.SetString("EmailRecuperacion", email);

            // Se muestra el código simulado
            ViewBag.SuccessMessage = $"Código generado: {codigo}";
            ViewBag.Redireccionar = true; // <- para activar el auto-redirect

            return View();
        }

        [HttpGet]
        public IActionResult RecuperarContrasenia()
        {
            // Solo muestra el formulario sin mensajes
            return View();
        }

        //// Acción para procesar la recuperación de contraseña
        //[HttpPost]
        //public IActionResult RecuperarContrasenia(string email)
        //{
        //    if (string.IsNullOrEmpty(email))
        //    {
        //        ViewBag.ErrorMessage = "El correo electrónico es requerido.";
        //        return View();
        //    }

        //    var recoveryCode = GenerateRecoveryCode();

        //    _emailService.SendPasswordRecoveryEmail(email, recoveryCode);

        //    ViewBag.SuccessMessage = "Se ha enviado un código de recuperación a tu correo.";

        //    return View();
        //}

        // Método para generar un código aleatorio de recuperación
        //private string GenerateRecoveryCode()
        //{
        //    var random = new Random();
        //    return random.Next(100000, 999999).ToString(); // Código de 6 dígitos
        //}


        [HttpGet]
        public IActionResult ConfirmarCodigo()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ConfirmarCodigo(string codigoIngresado, string nuevaPassword, string confirmarPassword)
        {
            string codigoGuardado = HttpContext.Session.GetString("CodigoRecuperacion");

            if (!string.IsNullOrEmpty(nuevaPassword) && !string.IsNullOrEmpty(confirmarPassword))
            {
                if (nuevaPassword != confirmarPassword)
                {
                    ViewBag.ErrorMessage = "Las contraseñas no coinciden.";
                    ViewBag.CodigoValidado = true;
                    return View();
                }

                HttpContext.Session.Remove("CodigoRecuperacion");

                ViewBag.SuccessMessage = "La contraseña fue cambiada exitosamente";
                return View();
            }

            if (codigoIngresado == codigoGuardado)
            {
                ViewBag.CodigoValidado = true;
                return View();
            }
            else
            {
                ViewBag.ErrorMessage = "El código ingresado es incorrecto.";
                return View();
            }
        }

    }
}
