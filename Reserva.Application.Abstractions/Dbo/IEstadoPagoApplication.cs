using Reserva.Dto.Base;
using Reserva.Dto.Dbo.EstadoPago;

namespace Reserva.Application.Abstractions.Dbo
{
    public interface IEstadoPagoApplication
    {
        Task<ResponseDto<GetEstadoPagoDto>> Create(CreateEstadoPagoDto createDto);
        Task<ResponseDto<GetEstadoPagoDto>> Update(UpdateEstadoPagoDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetEstadoPagoDto>> Get(int id);
        Task<ResponseDto<IEnumerable<ListEstadoPagoDto>>> List(int id);
        Task<ResponseDto<SearchResultDto<SearchEstadoPagoDto>>> Search(SearchParamsDto<SearchEstadoPagoFilterDto> searchParams);
        Task<ResponseDto<IEnumerable<SelectComboEstadoPagoDto>>> SelectCombo();
        Task<ResponseDto<SearchResultDto<SelectEstadoPagoDto>>> Select(SearchParamsDto<SelectEstadoPagoFilterDto> searchParams);

    }
}

