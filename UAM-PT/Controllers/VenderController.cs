using DAL;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Signers;
using System.Data.Entity.SqlServer;
using System.Drawing;
using System.Runtime.ConstrainedExecution;
using UAM_PT.Models;
using static System.Net.Mime.MediaTypeNames;

namespace UAM_PT.Controllers
{
    public class VenderController : Controller
    {
        public IActionResult PerfilVendedor()
        {
            var vendedorId = HttpContext.Session.GetString("VendedorId");

            if (string.IsNullOrEmpty(vendedorId))
            {
                TempData["MensajeVendedor"] = "Debes registrarte como vendedor para acceder a la sección de 'Vender'";
                return RedirectToAction("ConfiguracionCuenta", "Cuenta"); 
            }

            return View();
        }

        [HttpPost]
        public ActionResult AgregarProducto([FromForm] ProductoModel Datosproducto, IFormFile Imagen)
        {

            byte[] imagenBytes = null;
            #region comentario
            //if (Imagen != null && Imagen.Length > 0)
            //{
            //    using (var ms = new MemoryStream())
            //    {
            //        Imagen.CopyTo(ms);
            //        imagenBytes = ms.ToArray();
            //    }
            //}
            #endregion

            using (var stream = Imagen.OpenReadStream())
            using (var image = System.Drawing.Image.FromStream(stream))
            {
                // Crear un tamaño cuadrado (por ejemplo, 300x300)
                int size = 300;
                var thumb = new Bitmap(size, size);

                using (var graphics = Graphics.FromImage(thumb))
                {
                    graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
                    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

                    graphics.DrawImage(image, 0, 0, size, size);
                }

                using (var ms = new MemoryStream())
                {
                    var qualityParam = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 70L);
                    var jpegCodec = System.Drawing.Imaging.ImageCodecInfo.GetImageDecoders().First(c => c.FormatID == System.Drawing.Imaging.ImageFormat.Jpeg.Guid);
                    var encoderParams = new System.Drawing.Imaging.EncoderParameters(1);
                    encoderParams.Param[0] = qualityParam;

                    thumb.Save(ms, jpegCodec, encoderParams);
                    imagenBytes = ms.ToArray();
                }
            }


            try
            {
                using (NovaMerEntities2 db = new NovaMerEntities2())
                {
                    Guid productoID = Guid.NewGuid();
                    Producto RegistraProducto = new Producto();
                    RegistraProducto.ID = productoID;
                    RegistraProducto.Nombre = Datosproducto.Nombre.Trim();
                    RegistraProducto.VendedorID = Datosproducto.VendedorID;
                    RegistraProducto.CategoriaID = Datosproducto.CategoriaID;
                    RegistraProducto.SKU = Datosproducto.SKU;
                    RegistraProducto.Precio = Datosproducto.Precio;
                    RegistraProducto.Peso = Datosproducto.Peso;
                    RegistraProducto.Descripcion = Datosproducto.Descripcion;
                    RegistraProducto.Stock = Datosproducto.Stock;
                    RegistraProducto.Activo = true;
                    RegistraProducto.FechaRegistro = DateTime.Now;
                    RegistraProducto.Imagen = null;


                    db.Productoes.Add(RegistraProducto);

                    if (imagenBytes != null)
                    {
                        ImgProducto nuevaImagen = new ImgProducto
                        {
                            ID = Guid.NewGuid(),
                            ProductoID = productoID,
                            Imagen = imagenBytes,
                            FechaRegistro = DateTime.Now,
                            Activo = true
                        };

                        db.ImgProductoes.Add(nuevaImagen);
                    }

                    db.SaveChanges();
                }

                return Json(new { success = true, response_msg = "Producto guardado correctamente", code_msg = 1 });

            }
            catch (Exception)
            {
                return Json(new { success = false, response_msg = "Error", code_msg = -1 });
            }
        }

        public ActionResult traeCategorias()
        {
            try
            {
                using (var ctx = new NovaMerEntities2())
                {
                    var categorias = ctx.Categorias
                        .Where(x => x.Activo == true)
                        .Select(x => new
                        {
                            x.ID,
                            x.Nombre,
                            x.Descripcion
                        })
                        .ToList();

                    return Json(categorias);
                }
            }
            catch
            {
                return Json(new { success = false, response_msg = "Error", code_msg = -1 });
            } 
        }

        public ActionResult ObtenProductosPorUsuarioID(Guid usuarioId)
        {
            using (var ctx = new NovaMerEntities2())
            {
                var vendedor = ctx.Vendedors.FirstOrDefault(v => v.UsuarioID == usuarioId);

                if (vendedor == null)
                {
                    return Json(new { error = "No se encontró el vendedor" });
                }

                var productos = ctx.Productoes
                    .Where(x => x.VendedorID == vendedor.ID && x.Activo == true)
                    .ToList()
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
                        ImagenUrl = "/Vender/ObtenerImagen?id=" + x.ID + "&v=" + x.FechaRegistro.Ticks
                    });

                return Json(productos);
            }
        }

        public ActionResult ObtenProductosInactivos(Guid usuarioId)
        {
            using (var ctx = new NovaMerEntities2())
            {
                var vendedor = ctx.Vendedors.FirstOrDefault(v => v.UsuarioID == usuarioId);

                if (vendedor == null)
                {
                    return Json(new { error = "No se encontró el vendedor" });
                }

                var productos = ctx.Productoes
                    .Where(x => x.VendedorID == vendedor.ID && x.Activo == false)
                    .ToList()
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
                        ImagenUrl = "/Vender/ObtenerImagen?id=" + x.ID + "&v=" + x.FechaRegistro.Ticks
                    });

                return Json(productos);
            }
        }


        public ActionResult ObtenerImagen(Guid id)
        {
            using (var db = new NovaMerEntities2())
            {
                var imagenProducto = db.ImgProductoes
                    .Where(img => img.ProductoID == id && img.Activo == true)
                    .OrderByDescending(img => img.FechaRegistro)
                    .FirstOrDefault();

                if (imagenProducto != null && imagenProducto.Imagen != null)
                {
                    return File(imagenProducto.Imagen, "image/jpeg");
                }
            }

            return File(("~/images/default.jpg"), "image/jpeg");
        }


        public ActionResult ObtenerProductoPorId(Guid id)
        {
            using (var ctx = new NovaMerEntities2())
            {
                var producto = ctx.Productoes
                    .Where(p => p.ID == id && p.Activo == true)
                    .Select(p => new
                    {
                        p.ID,
                        p.Nombre,
                        p.Precio,
                        p.Stock,
                        p.Peso,
                        p.Descripcion,
                        ImagenUrl = "/Vender/ObtenerImagen?id=" + p.ID
                    })
                    .FirstOrDefault();

                if (producto == null)
                {
                    return Json(new { error = "Producto no encontrado" });
                }

                return Json(producto);
            }
        }

        [HttpPost]
        public ActionResult EditarProductoporId(Producto modelo, IFormFile Imagen)
        {
            using (var ctx = new NovaMerEntities2())
            {
                var producto = ctx.Productoes.FirstOrDefault(p => p.ID == modelo.ID && p.Activo == true);
                if (producto == null)
                {
                    return Json(new { success = false, message = "Producto no encontrado" });
                }

                // Actualizar propiedades
                producto.Nombre = modelo.Nombre;
                producto.Descripcion = modelo.Descripcion;
                producto.Precio = modelo.Precio;
                producto.Stock = modelo.Stock;
                producto.Peso = modelo.Peso;
                producto.FechaRegistro = DateTime.Now;

                // Imagen nueva (opcional)
                if (Imagen != null && Imagen.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        Imagen.OpenReadStream().CopyTo(ms);
                        var bytes = ms.ToArray();


                        // Desactivar imágenes anteriores
                        var anteriores = ctx.ImgProductoes.Where(i => i.ProductoID == producto.ID && i.Activo == true);
                        foreach (var img in anteriores)
                        {
                            img.Activo = false;
                        }

                        // Agregar imagen nueva
                        ctx.ImgProductoes.Add(new ImgProducto
                        {
                            ID = Guid.NewGuid(),
                            ProductoID = producto.ID,
                            Imagen = bytes,
                            Activo = true,
                            FechaRegistro = DateTime.Now
                        });
                    }
                }

                ctx.SaveChanges();
                return Json(new { success = true, message = "Producto actualizado correctamente" });
            }
        }

        public ActionResult InactivarProducto(Guid productoId)
        {
            using (var ctx = new NovaMerEntities2())
            {
                var producto = ctx.Productoes.FirstOrDefault(p => p.ID == productoId && p.Activo == true);
                if (producto == null)
                {
                    return Json(new { success = false, message = "Producto no encontrado" });
                }

                
                producto.Activo = false;

                

                ctx.SaveChanges();
                return Json(new { success = true, message = "Producto deshabilitado correctamente" });
            }
        }

        public ActionResult ActivarProducto(Guid productoId)
        {
            using (var ctx = new NovaMerEntities2())
            {
                var producto = ctx.Productoes.FirstOrDefault(p => p.ID == productoId && p.Activo == false);
                if (producto == null)
                {
                    return Json(new { success = false, message = "Producto no encontrado" });
                }

                producto.Activo = true;

                ctx.SaveChanges();
                return Json(new { success = true, message = "Producto habilitado correctamente" });
            }
        }

    }
}
