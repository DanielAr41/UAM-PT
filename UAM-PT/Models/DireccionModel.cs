namespace UAM_PT.Models
{
    public class DireccionModel
    {
        public Guid ID { get; set; }
        public Guid UsuarioID { get; set; }
        public string? Calle {  get; set; }
        public string? Numero { get; set; }
        public string? Localidad { get; set; }
        public string? Municipio { get; set; }
        public string? Estado { get; set; }
        public string? CodigoPostal { get; set; }
        public string? Pais { get; set; }
        public int? NumeroInt {  get; set; }
        public DateTime FechaRegistro {  get; set; }
        public bool Activo { get; set; }



    }
}
