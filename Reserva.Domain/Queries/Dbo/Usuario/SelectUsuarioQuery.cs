using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Usuario;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.Usuario
{
    public class SelectUsuarioQuery : SearchQueryBase<SelectUsuarioFilterDto, SelectUsuarioDto>
    {
        public SelectUsuarioQuery(SearchParamsDto<SelectUsuarioFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
