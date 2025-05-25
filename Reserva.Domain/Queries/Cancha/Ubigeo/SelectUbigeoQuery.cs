using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Ubigeo;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.Ubigeo
{
    public class SelectUbigeoQuery : SearchQueryBase<SelectUbigeoFilterDto, SelectUbigeoDto>
    {
        public SelectUbigeoQuery(SearchParamsDto<SelectUbigeoFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
