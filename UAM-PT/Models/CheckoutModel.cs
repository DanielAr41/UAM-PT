namespace UAM_PT.Models
{
    public class CheckoutModel
    {
        public List<CarritoProductoModel>? Productos { get; set; }
        public decimal Subtotal => Productos?.Sum(p => p.Cantidad * p.PrecioUnitario) ?? 0;
        public decimal Envio { get; set; } = 100;
        public decimal Total => Subtotal + Envio;

        // Datos del cliente
        public string? Nombre { get; set; }
        public string? ApellidoPaterno{ get; set; }
        public string? ApellidoMaterno { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }

        // Dirección
        public string? Direccion { get; set; }
        public string? Instrucciones { get; set; }

        // Selecciones
        public string? FechaEntrega { get; set; }
        public string? MetodoPago { get; set; }
    }
}
