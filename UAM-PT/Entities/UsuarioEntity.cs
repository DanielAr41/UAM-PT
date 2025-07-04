using DAL;
using System.ComponentModel.DataAnnotations;

namespace UAM_PT.Entities
{
    public class UsuarioEntity
    {
        public string Nombre { get; set; }
        public string Apaterno { get; set; }
        public string Amaterno { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string password {  get; set; }
    }
}
