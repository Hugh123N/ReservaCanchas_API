using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Provincia;

namespace Reserva.Application.Abstractions.Cancha
{
    public interface IProvinciaApplication
    {
        Task<ResponseDto<GetProvinciaDto>> Create(CreateProvinciaDto createDto);
        Task<ResponseDto<GetProvinciaDto>> Update(UpdateProvinciaDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetProvinciaDto>> Get(int id);
        Task<ResponseDto<IEnumerable<ListProvinciaDto>>> List(int id);
        Task<ResponseDto<SearchResultDto<SearchProvinciaDto>>> Search(SearchParamsDto<SearchProvinciaFilterDto> searchParams);
        Task<ResponseDto<IEnumerable<SelectComboProvinciaDto>>> SelectCombo();
        Task<ResponseDto<SearchResultDto<SelectProvinciaDto>>> Select(SearchParamsDto<SelectProvinciaFilterDto> searchParams);

    }
}

