using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DAL;
using System.Linq;
using UAM_PT.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Text.Json;

namespace UAM_PT.Controllers
{
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult InicioSesion()
        {
            return View();
        }

        public IActionResult Registro()
        {
            return View();
        }

        public IActionResult Login([FromBody] LoginModel login)
        {
            using (var ctx = new NovaMerEntities2())
            {
                var user = ctx.Usuarios.FirstOrDefault(x => x.Correo == login.usuario && x.password == login.password);
                Guid? vendedorID = null;

                if (user != null)
                {
                    var vendedor = ctx.Vendedors.FirstOrDefault(x => x.UsuarioID == user.ID);
                    if (vendedor != null)
                    {
                        vendedorID = vendedor.ID;
                    }

                    HttpContext.Session.SetString("UserId", user.ID.ToString());
                    if (vendedorID.HasValue)
                        HttpContext.Session.SetString("VendedorId", vendedorID.Value.ToString());
                }

                if (user == null)
                {
                    return Unauthorized(new { message = "Credenciales inválidas" });
                }

                var token = GenerateJwtToken(user.Correo);
                return Ok(new { token, usuarioId = user.ID, vendedorId = vendedorID});
            }
            #region codigo comentado
            //using (var ctx = new NovaMerEntities2())
            //{
            //    var auth = ctx.Usuarios
            //                  .Where(x => x.Correo == login.usuario && x.password == login.password)
            //                  .FirstOrDefault();

            //    if (auth != null)
            //    {
            //        var token = GenerateJwtToken(auth.Correo);
            //        return Json(new { success = true, message = "Inicio de sesión exitoso", token });
            //    }
            //    else
            //    {
            //        return Json(new { success = false, message = "Usuario o contraseña incorrectos" });
            //    }
            //}
            #endregion
        }

        [HttpPost]
        public JsonResult RegistrarUsuario([FromBody] RegistroModel registrar)
        {
            try
            {
                using (NovaMerEntities2 db = new NovaMerEntities2())
                {
                    Usuario Registra_Usuario = new Usuario();
                    Registra_Usuario.ID = Guid.NewGuid();
                    //Registra_Usuario.Nombre = nombre.Trim();
                    Registra_Usuario.Nombre = registrar.Nombre.Trim();
                    Registra_Usuario.Apaterno = registrar.Apaterno.Trim();
                    Registra_Usuario.Amaterno = registrar.Amaterno.Trim();
                    Registra_Usuario.Correo = registrar.Correo.Trim();
                    Registra_Usuario.password = registrar.pass.Trim();
                    Registra_Usuario.RolID = 3;
                    Registra_Usuario.Telefono = registrar.telefono.Trim();
                    Registra_Usuario.FechaRegistro = DateTime.Now;
                    Registra_Usuario.Activo = true;
                    db.Usuarios.Add(Registra_Usuario);
                    db.SaveChanges();
                }

                return Json(new { success = true, response_msg = "ok", code_msg = 1 });

            }
            catch (Exception)
            {
                return Json(new { success = false, response_msg = "Error", code_msg = -1 });
            }

        }

        private string GenerateJwtToken(string email)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, email)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost]
        public async Task<IActionResult> GoogleSimCallback([FromBody] JsonElement data)
        {
            var token = data.GetProperty("token").GetString();
            var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(token));

            var payload = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json);


            string email = payload["email"].ToString();
            string name = payload["name"].ToString();

            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Email, email),
                new Claim("provider", "google-sim")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Json(new { ok = true });
        }

        
        public IActionResult GoogleLogin()
        {
            return View();
        }
    }
}

