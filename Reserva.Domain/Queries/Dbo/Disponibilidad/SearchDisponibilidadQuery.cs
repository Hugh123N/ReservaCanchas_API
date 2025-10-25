using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Disponibilidad;

namespace Reserva.Domain.Queries.Dbo.Disponibilidad
{
    public class SearchDisponibilidadQuery : SearchQueryBase<SearchDisponibilidadFilterDto, SearchDisponibilidadDto>
    {
        public SearchDisponibilidadQuery(SearchParamsDto<SearchDisponibilidadFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
