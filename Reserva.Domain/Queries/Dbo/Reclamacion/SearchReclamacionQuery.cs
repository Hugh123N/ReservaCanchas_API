using Reserva.Domain.Queries.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Reclamacion;

namespace Reserva.Domain.Queries.Dbo.Reclamacion
{
    public class SearchReclamacionQuery : SearchQueryBase<SearchReclamacionFilterDto, SearchReclamacionDto>
    {
        public SearchReclamacionQuery(SearchParamsDto<SearchReclamacionFilterDto> searchParams) : base(searchParams)
        {
        }
    }
}
