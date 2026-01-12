using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Calendario;

namespace Reserva.Domain.Queries.Dbo.Calendario
{
    public class GetDisponibilidadSemanalQuery : QueryBase<DisponibilidadSemanalResponseDto>
    {
        public int IdCancha { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}
