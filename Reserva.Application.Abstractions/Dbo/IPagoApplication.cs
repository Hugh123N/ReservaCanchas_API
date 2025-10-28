using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Pago;

namespace Reserva.Application.Abstractions.Dbo
{
    public interface IPagoApplication
    {
        Task<ResponseDto<GetPagoDto>> Create(CreatePagoDto createDto);
        Task<ResponseDto<GetPagoDto>> Update(UpdatePagoDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetPagoDto>> Get(int id);
        Task<ResponseDto<IEnumerable<ListPagoDto>>> List(int id);
        Task<ResponseDto<SearchResultDto<SearchPagoDto>>> Search(SearchParamsDto<SearchPagoFilterDto> searchParams);
        Task<ResponseDto<IEnumerable<SelectComboPagoDto>>> SelectCombo();
        Task<ResponseDto<SearchResultDto<SelectPagoDto>>> Select(SearchParamsDto<SelectPagoFilterDto> searchParams);

        /// <summary>
        /// Confirma un pago ingresando el código de operación de Yape/Plin
        /// </summary>
        Task<ResponseDto<GetPagoDto>> ConfirmarPago(ConfirmarPagoDto confirmarDto);
    }
}

