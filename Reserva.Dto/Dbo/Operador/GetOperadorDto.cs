namespace Reserva.Dto.Dbo.Operador
{
    public class GetOperadorDto : OperadorDto
    {
        public int IdOperador { get; set; }
        public string? Imagen { get; set; }
        public DateTimeOffset? FechaCreacion { get; set; }
        public DateTimeOffset? FechaActualizacion { get; set; }
        public List<int>? CanchaIds { get; set; }
    }
}
