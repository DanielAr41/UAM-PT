namespace UAM_PT.Models
{
    public class VendedorModel
    {
        public Guid ID { get; set; }
        public Guid UsuarioID { get; set; }
        public string RFC {  get; set; }
        public byte[] IMGCuentaBanco { get; set; }
        public byte[] INE { get; set; }
        public byte[] ImgComprobanteDomicilio { get; set; }
    }
}
