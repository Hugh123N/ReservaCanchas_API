using Reserva.Dto.Base;
using Reserva.Dto.Cancha.EstadoUsuario;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.EstadoUsuario
{
    public class SelectEstadoUsuarioQuery : SearchQueryBase<SelectEstadoUsuarioFilterDto, SelectEstadoUsuarioDto>
    {
        public SelectEstadoUsuarioQuery(SearchParamsDto<SelectEstadoUsuarioFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
