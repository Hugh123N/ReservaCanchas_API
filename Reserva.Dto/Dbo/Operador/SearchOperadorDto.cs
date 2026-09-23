
using Reserva.Dto.Dbo.Cancha;

namespace Reserva.Dto.Dbo.Operador
{
    public class SearchOperadorDto : OperadorDto
    {
        public int IdOperador { get; set; }
        public string? Imagen { get; set; }
        public DateTimeOffset FechaCreacion { get; set; }
        public List<string>? Canchas { get; set; } = null!;
    }
}
