using Microsoft.AspNetCore.Mvc;
using UAM_PT.Services;

namespace UAM_PT.Controllers
{
    public class AccountController : Controller
    {
        private readonly EmailService _emailService;

        public AccountController(EmailService emailService)
        {
            _emailService = emailService;
        }

        // Acción para mostrar el formulario de recuperación de contraseña
        public IActionResult RecuperarContrasenia()
        {
            return View();
        }

        // Acción para procesar la recuperación de contraseña
        [HttpPost]
        public IActionResult RecuperarContrasenia(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ViewBag.ErrorMessage = "El correo electrónico es requerido.";
                return View();
            }

            // Generar un código de recuperación aleatorio
            var recoveryCode = GenerateRecoveryCode();

            // Enviar el correo
            _emailService.SendPasswordRecoveryEmail(email, recoveryCode);

            ViewBag.SuccessMessage = "Se ha enviado un código de recuperación a tu correo.";

            return View();
        }

        // Método para generar un código aleatorio de recuperación
        private string GenerateRecoveryCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString(); // Código de 6 dígitos
        }
    }
}
