namespace UAM_PT.Models
{
    public class PedidoModel
    {
        public Guid PedidoId { get; set; }
        public DateTime FechaPedido { get; set; }
        public string? Estatus { get; set; }
        public int TotalProductos { get; set; }
        public List<ProductoPedidoModel>? Productos { get; set; }

        public int PasoActual
        {
            get
            {
                return Estatus switch
                {
                    "Pendiente" => 1,
                    "Procesado" => 2,
                    "Enviado" => 2,
                    "Entregado" => 3,
                    _ => 1
                };
            }
        }
    }
}
