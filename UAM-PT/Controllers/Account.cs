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

            bool existeUsuario = usuario == null ? false : true;

            if (!existeUsuario)
            {
                ViewBag.ErrorMessage = "El correo no está registrado";
                return View();
            }

            string codigo = new Random().Next(100000, 999999).ToString();


            HttpContext.Session.SetString("CodigoRecuperacion", codigo);
            HttpContext.Session.SetString("EmailRecuperacion", email);

            // No se envía correo real, solo se muestra el mensaje
            ViewBag.SuccessMessage = $"Código generado: {codigo}";

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

            // Si vienen passwords significa que ya validamos y ahora estamos cambiando
            if (!string.IsNullOrEmpty(nuevaPassword) && !string.IsNullOrEmpty(confirmarPassword))
            {
                if (nuevaPassword != confirmarPassword)
                {
                    ViewBag.ErrorMessage = "Las contraseñas no coinciden.";
                    ViewBag.CodigoValidado = true; // para volver a mostrar inputs de contraseña
                    return View();
                }

                // aquí simulas guardado en BD
                // luego borras codigo de sesión
                HttpContext.Session.Remove("CodigoRecuperacion");

                ViewBag.SuccessMessage = "La contraseña fue cambiada exitosamente";
                return View();
            }

            // Validación del código
            if (codigoIngresado == codigoGuardado)
            {
                ViewBag.CodigoValidado = true; // señal a la vista para mostrar inputs
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
