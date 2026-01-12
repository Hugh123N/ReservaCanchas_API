using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Calendario;

namespace Reserva.Domain.Queries.Dbo.Calendario
{
    /// <summary>
    /// Query para validar la disponibilidad de horarios
    /// </summary>
    public class ValidarDisponibilidadQuery : QueryBase<ValidarDisponibilidadResponseDto>
    {
        /// <summary>
        /// ID de la cancha
        /// </summary>
        public int IdCancha { get; set; }

        /// <summary>
        /// Lista de bloques de horarios a validar
        /// </summary>
        public List<BloqueHorarioDto> Horarios { get; set; } = new List<BloqueHorarioDto>();
    }
}
