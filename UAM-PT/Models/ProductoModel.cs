using DAL;

namespace UAM_PT.Models
{
    public class ProductoModel
    {
        public Guid ID { get; set; }
        public Guid VendedorID { get; set; }
        public Guid CategoriaID { get; set; }
        public string? SKU { get; set; }
        public string? Nombre { get; set; }
        public decimal Precio { get; set; }
        public decimal Peso { get; set; }
        public string? Descripcion { get; set; }
        public int Stock { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaRegistro { get; set; }
        public byte[]? Imagen { get; set; }
        public List<Producto>? Relacionados { get; set; }
    }
}
