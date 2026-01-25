using Reserva.Dto.Dbo.Calendario;
using Reserva.Dto.Dbo.Pago;

namespace Reserva.Dto.Dbo.Reserva
{
    public class UpdateReservaDto : ReservaDto
    {
        public int IdReserva { get; set; }
        public string? CodigoEstadoReserva { get; set; }
        public List<BloqueHorarioDto> Horarios { get; set; } = new List<BloqueHorarioDto>();
        public List<UpdatePagoDto> PagoReserva { get; set; } = new List<UpdatePagoDto>();
    }
}
