namespace UAM_PT.Models
{
    public class CarritoProductoModel
    {
        public Guid ProductoId { get; set; }
        public string Nombre { get; set; }
        public string Imagen { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal => PrecioUnitario * Cantidad;
    }
}
