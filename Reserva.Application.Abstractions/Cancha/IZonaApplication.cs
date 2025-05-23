using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Zona;

namespace Reserva.Application.Abstractions.Cancha
{
    public interface IZonaApplication
    {
        Task<ResponseDto<GetZonaDto>> Create(CreateZonaDto createDto);
        Task<ResponseDto<GetZonaDto>> Update(UpdateZonaDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetZonaDto>> Get(int id);
        Task<ResponseDto<IEnumerable<ListZonaDto>>> List(int id);
        Task<ResponseDto<SearchResultDto<SearchZonaDto>>> Search(SearchParamsDto<SearchZonaFilterDto> searchParams);
        Task<ResponseDto<IEnumerable<SelectComboZonaDto>>> SelectCombo();
        Task<ResponseDto<SearchResultDto<SelectZonaDto>>> Select(SearchParamsDto<SelectZonaFilterDto> searchParams);

    }
}

