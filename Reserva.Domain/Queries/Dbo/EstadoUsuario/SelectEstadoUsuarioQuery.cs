using Reserva.Dto.Base;
using Reserva.Dto.Dbo.EstadoUsuario;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.EstadoUsuario
{
    public class SelectEstadoUsuarioQuery : SearchQueryBase<SelectEstadoUsuarioFilterDto, SelectEstadoUsuarioDto>
    {
        public SelectEstadoUsuarioQuery(SearchParamsDto<SelectEstadoUsuarioFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
