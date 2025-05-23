using Reserva.Dto.Base;
using Reserva.Dto.Cancha.MetodoPago;

namespace Reserva.Application.Abstractions.Cancha
{
    public interface IMetodoPagoApplication
    {
        Task<ResponseDto<GetMetodoPagoDto>> Create(CreateMetodoPagoDto createDto);
        Task<ResponseDto<GetMetodoPagoDto>> Update(UpdateMetodoPagoDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetMetodoPagoDto>> Get(int id);
        Task<ResponseDto<IEnumerable<ListMetodoPagoDto>>> List(int id);
        Task<ResponseDto<SearchResultDto<SearchMetodoPagoDto>>> Search(SearchParamsDto<SearchMetodoPagoFilterDto> searchParams);
        Task<ResponseDto<IEnumerable<SelectComboMetodoPagoDto>>> SelectCombo();
        Task<ResponseDto<SearchResultDto<SelectMetodoPagoDto>>> Select(SearchParamsDto<SelectMetodoPagoFilterDto> searchParams);

    }
}

