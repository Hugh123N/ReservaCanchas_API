namespace Reserva.Dto.Dbo.Operador
{
    public class GetOperadorDto : OperadorDto
    {
        public int IdOperador { get; set; }
        public string? Nombre { get; set; } = null!;
        public string? Apellidos { get; set; } = null!;
        public string? Email { get; set; } = null!;
        public string? Telefono { get; set; }
        public string? Imagen { get; set; }
        public DateTimeOffset? FechaCreacion { get; set; }
        public DateTimeOffset? FechaActualizacion { get; set; }
        public List<int>? CanchaIds { get; set; }
    }
}
