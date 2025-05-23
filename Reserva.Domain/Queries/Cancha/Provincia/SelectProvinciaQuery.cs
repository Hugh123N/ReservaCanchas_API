using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Provincia;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.Provincia
{
    public class SelectProvinciaQuery : SearchQueryBase<SelectProvinciaFilterDto, SelectProvinciaDto>
    {
        public SelectProvinciaQuery(SearchParamsDto<SelectProvinciaFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
