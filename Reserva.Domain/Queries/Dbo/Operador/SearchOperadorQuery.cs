using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Operador;

namespace Reserva.Domain.Queries.Dbo.Operador
{
    public class SearchOperadorQuery : SearchQueryBase<SearchOperadorFilterDto, SearchOperadorDto>
    {
        public SearchOperadorQuery(SearchParamsDto<SearchOperadorFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
