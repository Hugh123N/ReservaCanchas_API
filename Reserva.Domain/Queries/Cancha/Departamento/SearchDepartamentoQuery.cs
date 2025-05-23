using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Departamento;

namespace Reserva.Domain.Queries.Cancha.Departamento
{
    public class SearchDepartamentoQuery : SearchQueryBase<SearchDepartamentoFilterDto, SearchDepartamentoDto>
    {
        public SearchDepartamentoQuery(SearchParamsDto<SearchDepartamentoFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
