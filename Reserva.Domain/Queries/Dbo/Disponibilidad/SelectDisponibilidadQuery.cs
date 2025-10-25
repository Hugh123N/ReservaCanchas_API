using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Disponibilidad;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.Disponibilidad
{
    public class SelectDisponibilidadQuery : SearchQueryBase<SelectDisponibilidadFilterDto, SelectDisponibilidadDto>
    {
        public SelectDisponibilidadQuery(SearchParamsDto<SelectDisponibilidadFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
