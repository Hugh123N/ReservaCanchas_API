using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Disponibilidad;

namespace Reserva.Domain.Queries.Cancha.Disponibilidad
{
    public class SearchDisponibilidadQuery : SearchQueryBase<SearchDisponibilidadFilterDto, SearchDisponibilidadDto>
    {
        public SearchDisponibilidadQuery(SearchParamsDto<SearchDisponibilidadFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
