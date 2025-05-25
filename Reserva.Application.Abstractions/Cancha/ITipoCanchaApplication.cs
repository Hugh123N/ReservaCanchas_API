using Reserva.Dto.Base;
using Reserva.Dto.Cancha.TipoCancha;

namespace Reserva.Application.Abstractions.Cancha
{
    public interface ITipoCanchaApplication
    {
        Task<ResponseDto<GetTipoCanchaDto>> Create(CreateTipoCanchaDto createDto);
        Task<ResponseDto<GetTipoCanchaDto>> Update(UpdateTipoCanchaDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetTipoCanchaDto>> Get(int id);
        Task<ResponseDto<IEnumerable<ListTipoCanchaDto>>> List(int id);
        Task<ResponseDto<SearchResultDto<SearchTipoCanchaDto>>> Search(SearchParamsDto<SearchTipoCanchaFilterDto> searchParams);
        Task<ResponseDto<IEnumerable<SelectComboTipoCanchaDto>>> SelectCombo();
        Task<ResponseDto<SearchResultDto<SelectTipoCanchaDto>>> Select(SearchParamsDto<SelectTipoCanchaFilterDto> searchParams);

    }
}

