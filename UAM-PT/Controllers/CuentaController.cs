using DAL;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;
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

    }
}
