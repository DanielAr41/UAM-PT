namespace UAM_PT.Models
{
    public class ImgProductoModel
    {
        public Guid ID { get; set; }
        public Guid ProductoID { get; set; }
        public byte[] Imagen { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Activo { get; set; }
    }
}
