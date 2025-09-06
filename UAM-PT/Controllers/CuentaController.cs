using DAL;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Data.Entity;
using System.Data.Entity.Validation;
using UAM_PT.Models;

namespace UAM_PT.Controllers
{
    public class CuentaController : Controller
    {
        public IActionResult ConfiguracionCuenta()
        {
            return View();
        }

        [HttpGet]
        public ActionResult InfoUsuarioPorID(Guid usuarioId)
        {
            using (var ctx = new NovaMerEntities2())
            {
                var usuario = ctx.Usuarios
                    .Where(x => x.ID == usuarioId && x.Activo == true)
                    .Select(x => new
                    {
                        x.ID,
                        x.Nombre,
                        x.Apaterno,
                        x.Amaterno,
                        x.Telefono,
                        x.Correo
                    })
                    .FirstOrDefault();

                if (usuario == null)
                {
                    return Json(new { success = false, message = "Usuario no encontrado" });
                }

                return Json(new { success = true, data = usuario });
            }
        }

        public ActionResult AgregarDireccion([FromForm] DireccionModel DatosDireccion, Guid uid)
        {
            try
            {
                using (NovaMerEntities2 db = new NovaMerEntities2())
                {
                    Guid direccionId = Guid.NewGuid();
                    Direccione RegistraDireccion = new Direccione();
                    RegistraDireccion.ID = direccionId;
                    RegistraDireccion.UsuarioID = uid;
                    RegistraDireccion.Calle = DatosDireccion.Calle;
                    RegistraDireccion.Numero = DatosDireccion.Numero;
                    RegistraDireccion.Localidad = DatosDireccion.Localidad;
                    RegistraDireccion.Municipio = DatosDireccion.Municipio;
                    RegistraDireccion.Estado = DatosDireccion.Estado;
                    RegistraDireccion.CodigoPostal = DatosDireccion.CodigoPostal;
                    RegistraDireccion.Pais = DatosDireccion.Pais;
                    RegistraDireccion.NumeroInt = DatosDireccion.NumeroInt;
                    RegistraDireccion.Activo = true;
                    RegistraDireccion.FechaRegistro = DateTime.Now;


                    db.Direcciones.Add(RegistraDireccion);

                    db.SaveChanges();
                }

                return Json(new { success = true, response_msg = "Direccion guardada correctamente", code_msg = 1 });

            }
            catch (Exception)
            {
                return Json(new { success = false, response_msg = "Error", code_msg = -1 });
            }
        }


        public ActionResult CargaDireccion(Guid usuarioId)
        {
            using (var ctx = new NovaMerEntities2())
            {
                var direccion = ctx.Direcciones
                    .Where(x => x.UsuarioID == usuarioId && x.Activo == true && x.Predeterminada == true)
                    .Select(x => new
                    {
                        x.ID,
                        x.Calle,
                        x.Numero,
                        x.Localidad,
                        x.Municipio,
                        x.Estado,
                        x.CodigoPostal,
                        x.Pais,
                        x.NumeroInt,
                        x.FechaRegistro
                    })
                    .FirstOrDefault();

                if (direccion == null)
                {
                    return Json(new { success = false, message = "Sin direcciones" });
                }

                return Json(new { success = true, data = direccion });
            }
        }

        public ActionResult guardaInformacionUsuario([FromForm] UsuarioModel DatosUsuario, Guid uid)
        {
            try
            {
                using (NovaMerEntities2 db = new NovaMerEntities2())
                {
                    var actualizaUsuario = db.Usuarios.FirstOrDefault(u => u.ID == uid);

                    if (actualizaUsuario == null)
                    {
                        return Json(new { success = false, response_msg = "Usuario no encontrado", code_msg = -2 });
                    }

                    actualizaUsuario.Nombre = DatosUsuario.Nombres;
                    actualizaUsuario.Apaterno = DatosUsuario.Apaterno;
                    actualizaUsuario.Amaterno = DatosUsuario.Amaterno;
                    actualizaUsuario.Telefono = DatosUsuario.Telefono;
                    actualizaUsuario.Correo = DatosUsuario.Correo;

                    db.SaveChanges();
                }

                return Json(new { success = true, response_msg = "Información guardada correctamente", code_msg = 1 });

            }
            catch (Exception)
            {
                return Json(new { success = false, response_msg = "Error al guardar el usuario", code_msg = -1 });
            }
        }

        public JsonResult MarcarComoPredeterminada(Guid direccionId, Guid usuarioId)
        {
            try
            {
                using (var db = new NovaMerEntities2())
                {
                    // Desactivar todas las direcciones del usuario
                    var direcciones = db.Direcciones.Where(d => d.UsuarioID == usuarioId);
                    foreach (var dir in direcciones)
                    {
                        dir.Predeterminada = false;
                    }
                    db.SaveChanges();

                    // Activar la dirección predeterminada seleccionada
                    var direccionPred = db.Direcciones.FirstOrDefault(d => d.ID == direccionId);
                    if (direccionPred != null)
                    {
                        direccionPred.Predeterminada = true;
                        db.SaveChanges();

                        return Json(new { success = true });
                    }

                    return Json(new { success = false, message = "Dirección no encontrada" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al actualizar: " + ex.Message });
            }
        }

        public ActionResult TraeDireccionesPorIdUsuario(Guid usuarioId)
        {
            using (var db = new NovaMerEntities2())
            {
                var direcciones = db.Direcciones
                    .Where(d => d.UsuarioID == usuarioId)
                    .Select(d => new
                    {
                        id = d.ID,
                        texto = d.Calle + " " + d.Numero + ", " + d.Localidad + ", " + d.Municipio + ", " + d.Estado + ", " + d.CodigoPostal,
                        Predeterminada = d.Predeterminada
                    })
                    .ToList();

                return Json(new { direcciones, success = true } );
            }
        }

        public ActionResult AgregarMetodoPago(MetodoPagoUsuarioModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    
                    var userIdString = HttpContext.Session.GetString("UserId");
                    if (!string.IsNullOrEmpty(userIdString))
                    {
                        model.UsuarioId = Guid.Parse(userIdString);
                    }
                    model.Activo = true;

                    using (var db = new NovaMerEntities2())
                    {
                        var entidad = new MetodosPagoUsuario
                        {
                            UsuarioId = model.UsuarioId,
                            MetodoPagoId = model.MetodoPagoId,
                            NumeroTarjeta = model.NumeroTarjeta,
                            FechaVencimiento = model.FechaVencimiento,
                            NombreTitular = model.NombreTitular,
                            CuentaPaypal = model.CuentaPaypal,
                            CuentaMercadoPago = model.CuentaMercadoPago,
                            Activo = true
                        };

                        db.MetodosPagoUsuarios.Add(entidad);
                        db.SaveChanges();
                    }


                    return Json(new { success = true, message = "Método de pago agregado correctamente." });
                }

                return Json(new { success = false, message = "Datos inválidos." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpGet]
        public JsonResult ObtenerMetodosPago()
        {
            Guid userId = Guid.Parse(HttpContext.Session.GetString("UserId"));

            NovaMerEntities2 db = new NovaMerEntities2();
            var metodos = db.MetodosPagoUsuarios
                            .Where(m => m.UsuarioId == userId && m.Activo)
                            .Select(m => new {
                                m.Id,
                                Descripcion = m.NumeroTarjeta ?? m.CuentaPaypal ?? m.CuentaMercadoPago ?? "Otro método",
                                EsPredeterminado = m.EsPredeterminado
                            }).ToList();

            return Json(metodos);
        }

        [HttpPost]
        public JsonResult EstablecerPredeterminado(int metodoId)
        {
            Guid userId = Guid.Parse(HttpContext.Session.GetString("UserId"));
            NovaMerEntities2 db = new NovaMerEntities2();
            var metodos = db.MetodosPagoUsuarios.Where(m => m.UsuarioId == userId).ToList();
            foreach (var m in metodos)
                m.EsPredeterminado = (m.Id == metodoId);

            db.SaveChanges();
            return Json(new { success = true });
        }

        [HttpGet]
        public JsonResult ObtenerMetodoPredeterminado()
        {
            Guid userId = Guid.Parse(HttpContext.Session.GetString("UserId"));
            NovaMerEntities2 db = new NovaMerEntities2();
            var metodo = db.MetodosPagoUsuarios
                           .Where(m => m.UsuarioId == userId && m.EsPredeterminado)
                           .Select(m => new {
                               m.NumeroTarjeta,
                               m.CuentaPaypal,
                               m.CuentaMercadoPago
                           })
                           .FirstOrDefault();

            return Json(metodo);
        }

        [HttpPost]
        public JsonResult CambiarPassword([FromBody] CambioPasswordModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Datos inválidos" });

            Guid userId = Guid.Parse(HttpContext.Session.GetString("UserId"));
            NovaMerEntities2 db = new NovaMerEntities2();
            var usuario = db.Usuarios.Find(userId);
            if (usuario == null)
                return Json(new { success = false, message = "Usuario no encontrado" });

            // Validar contraseña actual
            if (usuario.password != model.PasswordActual)
                return Json(new { success = false, message = "Contraseña actual incorrecta" });

            // Guardar nueva contraseña
            usuario.password = model.PasswordNueva; 
            db.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult RegistrarVendedor(string rfc, string curp, string cuentaBancaria, IFormFile identificacion, IFormFile comprobanteDomicilio)
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            try
            {
                if (string.IsNullOrEmpty(userIdString))
                {
                    return Json(new { success = false, response_msg = "Usuario no autenticado", code_msg = -2 });
                }


                using (NovaMerEntities2 db = new NovaMerEntities2())
                {
                    Guid userId = Guid.Parse(userIdString);


                    Vendedor vendedor = new Vendedor
                    {
                        ID = Guid.NewGuid(),
                        UsuarioID = userId,
                        RFC = rfc,
                        Curp = curp,
                        CuentaBancaria = cuentaBancaria,
                        Activo = true,
                        FechaRegistro = DateTime.Now,
                    };

                    if (identificacion != null && identificacion.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            identificacion.CopyTo(ms);
                            vendedor.INE = ms.ToArray();
                        }
                    }

                    if (comprobanteDomicilio != null && comprobanteDomicilio.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            comprobanteDomicilio.CopyTo(ms);
                            vendedor.ImgComprobanteDomicilio = ms.ToArray();
                        }
                    }

                    db.Vendedors.Add(vendedor);
                    db.SaveChanges();
                }

                return Json(new { success = true, response_msg = "Vendedor registrado correctamente", code_msg = 1 });

            }
            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
        .SelectMany(eve => eve.ValidationErrors)
        .Select(ve => $"Propiedad: {ve.PropertyName}, Error: {ve.ErrorMessage}");

                var fullErrorMessage = string.Join("; ", errorMessages);

                // Devuelve JSON para ver en el frontend
                return Json(new { success = false, response_msg = fullErrorMessage, code_msg = -1 });
            }
        }


    }
}
