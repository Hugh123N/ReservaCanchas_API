using Reserva.Dto.Base;
using Reserva.Dto.Cancha.CanchaFavorita;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.CanchaFavorita
{
    public class SelectCanchaFavoritaQuery : SearchQueryBase<SelectCanchaFavoritaFilterDto, SelectCanchaFavoritaDto>
    {
        public SelectCanchaFavoritaQuery(SearchParamsDto<SelectCanchaFavoritaFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
