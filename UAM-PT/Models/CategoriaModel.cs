namespace UAM_PT.Models
{
    public class CategoriaModel
    {
        public Guid ID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Acitvo {  get; set; }
    }
}
