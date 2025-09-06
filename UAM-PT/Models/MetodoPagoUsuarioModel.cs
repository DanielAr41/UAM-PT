namespace UAM_PT.Models
{
    public class MetodoPagoUsuarioModel
    {
        public Guid ID { get; set; }
        public Guid UsuarioId { get; set; }
        public int MetodoPagoId { get; set; }
        public string? NumeroTarjeta { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public string? NombreTitular { get; set; }
        public string? CuentaPaypal { get; set; }
        public string? CuentaMercadoPago { get; set; }
        public bool Activo { get; set; }
        public MetodoPagoModel? MetodoPago { get; set; }
    }
}
