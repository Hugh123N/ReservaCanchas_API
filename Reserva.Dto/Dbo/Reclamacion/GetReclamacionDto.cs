namespace Reserva.Dto.Dbo.Reclamacion
{
    public class GetReclamacionDto : ReclamacionDto
    {
        public int IdReclamacion { get; set; }
        public string NumeroHoja { get; set; } = "";
        public DateTimeOffset FechaRegistro { get; set; }
        public string ComercioTitular { get; set; } = "";
        public string ComercioNombre { get; set; } = "";
        public string ComercioRuc { get; set; } = "";
        public string ComercioDireccion { get; set; } = "";
        public string ComercioTelefono { get; set; } = "";
        public string ComercioEmail { get; set; } = "";
        public string Estado { get; set; } = "";
        public string? Observaciones { get; set; }
        public string? Respuesta { get; set; }
        public DateTimeOffset? FechaComunicacionRespuesta { get; set; }
        public string? ReferenciaRespuesta { get; set; }
        public string? ResponsableRespuesta { get; set; }
        public bool? CopiaEnviada { get; set; }
    }
}
