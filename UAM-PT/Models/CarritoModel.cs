namespace UAM_PT.Models
{
    public class CarritoModel
    {
        public List<CarritoProductoModel> Productos { get; set; } = new();
        public decimal Total => Productos.Sum(p => p.Subtotal);
    }
}
