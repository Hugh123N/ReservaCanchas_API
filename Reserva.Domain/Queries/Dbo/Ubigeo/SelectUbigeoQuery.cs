using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Ubigeo;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.Ubigeo
{
    public class SelectUbigeoQuery : SearchQueryBase<SelectUbigeoFilterDto, SelectUbigeoDto>
    {
        public SelectUbigeoQuery(SearchParamsDto<SelectUbigeoFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
