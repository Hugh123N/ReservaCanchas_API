using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Calendario;

namespace Reserva.Domain.Queries.Dbo.Calendario
{
    public class ValidarDisponibilidadQuery : QueryBase<ValidarDisponibilidadResponseDto>
    {
        public int IdCancha { get; set; }

        public List<BloqueHorarioDto> Horarios { get; set; } = new List<BloqueHorarioDto>();
    }
}
