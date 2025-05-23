using Reserva.Dto.Base;
using Reserva.Dto.Cancha.GananciaProveedor;

namespace Reserva.Application.Abstractions.Cancha
{
    public interface IGananciaProveedorApplication
    {
        Task<ResponseDto<GetGananciaProveedorDto>> Create(CreateGananciaProveedorDto createDto);
        Task<ResponseDto<GetGananciaProveedorDto>> Update(UpdateGananciaProveedorDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetGananciaProveedorDto>> Get(int id);
        Task<ResponseDto<IEnumerable<ListGananciaProveedorDto>>> List(int id);
        Task<ResponseDto<SearchResultDto<SearchGananciaProveedorDto>>> Search(SearchParamsDto<SearchGananciaProveedorFilterDto> searchParams);
        Task<ResponseDto<IEnumerable<SelectComboGananciaProveedorDto>>> SelectCombo();
        Task<ResponseDto<SearchResultDto<SelectGananciaProveedorDto>>> Select(SearchParamsDto<SelectGananciaProveedorFilterDto> searchParams);

    }
}

