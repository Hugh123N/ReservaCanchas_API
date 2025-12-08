using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Hora;

namespace Reserva.Application.Abstractions.Dbo
{
    public interface IHoraApplication
    {
        Task<ResponseDto<GetHoraDto>> Get(int id);
        Task<ResponseDto<IEnumerable<ListHoraDto>>> List(int id);
        Task<ResponseDto<SearchResultDto<SearchHoraDto>>> Search(SearchParamsDto<SearchHoraFilterDto> searchParams);
        Task<ResponseDto<IEnumerable<SelectComboHoraDto>>> SelectCombo();

    }
}

