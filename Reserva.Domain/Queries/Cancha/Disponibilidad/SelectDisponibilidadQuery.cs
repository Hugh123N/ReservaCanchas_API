using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Disponibilidad;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.Disponibilidad
{
    public class SelectDisponibilidadQuery : SearchQueryBase<SelectDisponibilidadFilterDto, SelectDisponibilidadDto>
    {
        public SelectDisponibilidadQuery(SearchParamsDto<SelectDisponibilidadFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
