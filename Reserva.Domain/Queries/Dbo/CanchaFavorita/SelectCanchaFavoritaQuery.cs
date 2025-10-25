using Reserva.Dto.Base;
using Reserva.Dto.Dbo.CanchaFavorita;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.CanchaFavorita
{
    public class SelectCanchaFavoritaQuery : SearchQueryBase<SelectCanchaFavoritaFilterDto, SelectCanchaFavoritaDto>
    {
        public SelectCanchaFavoritaQuery(SearchParamsDto<SelectCanchaFavoritaFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
