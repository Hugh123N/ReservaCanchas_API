namespace Reserva.Dto.Dbo.Reclamacion
{
    public class AtenderReclamacionDto
    {
        public string Estado { get; set; } = "";
        public string? Observaciones { get; set; }
        public string? Respuesta { get; set; }
        public DateTimeOffset? FechaComunicacionRespuesta { get; set; }
        public string? ReferenciaRespuesta { get; set; }
    }
}
