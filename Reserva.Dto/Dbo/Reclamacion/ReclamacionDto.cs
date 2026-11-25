namespace Reserva.Dto.Dbo.Reclamacion
{
    public class ReclamacionDto
    {
        public string Nombres { get; set; } = "";
        public string TipDoc { get; set; } = "";
        public string NroDoc { get; set; } = "";
        public string Domicilio { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Telefono { get; set; }
        public bool EsMenor { get; set; }
        public string? Representante { get; set; }
        public string DescripcionServicio { get; set; } = "";
        public decimal? MontoReclamado { get; set; }
        public string? ReferenciaOperacion { get; set; }
        public string Tipo { get; set; } = "";
        public string Detalle { get; set; } = "";
        public string Pedido { get; set; } = "";
    }
}
