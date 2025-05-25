using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Usuario;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.Usuario
{
    public class SelectUsuarioQuery : SearchQueryBase<SelectUsuarioFilterDto, SelectUsuarioDto>
    {
        public SelectUsuarioQuery(SearchParamsDto<SelectUsuarioFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
