using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Departamento;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.Departamento
{
    public class SelectDepartamentoQuery : SearchQueryBase<SelectDepartamentoFilterDto, SelectDepartamentoDto>
    {
        public SelectDepartamentoQuery(SearchParamsDto<SelectDepartamentoFilterDto> searchParams) : base(searchParams)
        {

        }
    }
}
