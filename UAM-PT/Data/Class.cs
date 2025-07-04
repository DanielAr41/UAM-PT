using DAL;
using UAM_PT.Entities;
using System.Linq;


namespace UAM_PT.Data
{
    public class UsuarioRepository
    {
        public List<UsuarioEntity> ObtenListaUsuarios(string correo, string password)
        {
            using (var ctx = new NovaMerEntities2())
            {
                return ctx.Usuarios
                    .Where(x => x.Correo == correo && x.password == password)
                    .Select(x => new UsuarioEntity
                    {
                        Nombre = x.Nombre,
                        Apaterno = x.Apaterno,
                        Amaterno = x.Amaterno,
                        Telefono = x.Telefono,
                        Correo = x.Correo,
                        password = x.password
                    }).ToList();
            }
        }
    }
}
