namespace UAM_PT.Models
{
    public class ProductoPedidoModel
    {
        public Guid ProductoId { get; set; }
        public string? Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
    }
}
