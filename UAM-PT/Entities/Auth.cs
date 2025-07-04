using DAL;
using System.ComponentModel.DataAnnotations;

namespace UAM_PT.Entities
{
    public class AuthEntity
    {
        public int Id { get; set; }
        public string Correo { get; set; }
        public string Password { get; set; }
        
    }
}
