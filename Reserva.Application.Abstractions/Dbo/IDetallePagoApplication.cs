using Reserva.Dto.Base;
using Reserva.Dto.Dbo.DetallePago;

namespace Reserva.Application.Abstractions.Dbo
{
    public interface IDetallePagoApplication
    {
        Task<ResponseDto<GetDetallePagoDto>> Create(CreateDetallePagoDto createDto);
        Task<ResponseDto<GetDetallePagoDto>> Update(UpdateDetallePagoDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetDetallePagoDto>> Get(int id);
        Task<ResponseDto<IEnumerable<ListDetallePagoDto>>> List(int id);
        Task<ResponseDto<SearchResultDto<SearchDetallePagoDto>>> Search(SearchParamsDto<SearchDetallePagoFilterDto> searchParams);
        Task<ResponseDto<IEnumerable<SelectComboDetallePagoDto>>> SelectCombo();
        Task<ResponseDto<SearchResultDto<SelectDetallePagoDto>>> Select(SearchParamsDto<SelectDetallePagoFilterDto> searchParams);

    }
}

