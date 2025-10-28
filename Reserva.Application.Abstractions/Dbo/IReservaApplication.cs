using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Reserva;

namespace Reserva.Application.Abstractions.Dbo
{
    public interface IReservaApplication
    {
        /// <summary>
        /// Crea una nueva reserva con pago pendiente y genera QR para Yape/Plin
        /// </summary>
        Task<ResponseDto<ReservaConPagoDto>> Create(CreateReservaDto createDto);
        Task<ResponseDto<GetReservaDto>> Update(UpdateReservaDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetReservaDto>> Get(int id);
        Task<ResponseDto<IEnumerable<ListReservaDto>>> List(int id);
        Task<ResponseDto<SearchResultDto<SearchReservaDto>>> Search(SearchParamsDto<SearchReservaFilterDto> searchParams);
        Task<ResponseDto<IEnumerable<SelectComboReservaDto>>> SelectCombo();
        Task<ResponseDto<SearchResultDto<SelectReservaDto>>> Select(SearchParamsDto<SelectReservaFilterDto> searchParams);

    }
}

