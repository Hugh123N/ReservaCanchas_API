using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Rol;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.Rol
{
    public class SelectRolQuery : SearchQueryBase<SelectRolFilterDto, SelectRolDto>
    {
        public SelectRolQuery(SearchParamsDto<SelectRolFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
