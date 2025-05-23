using Reserva.Dto.Base;
using Reserva.Dto.Cancha.TipoProveedor;

namespace Reserva.Application.Abstractions.Cancha
{
    public interface ITipoProveedorApplication
    {
        Task<ResponseDto<GetTipoProveedorDto>> Create(CreateTipoProveedorDto createDto);
        Task<ResponseDto<GetTipoProveedorDto>> Update(UpdateTipoProveedorDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetTipoProveedorDto>> Get(int id);
        Task<ResponseDto<IEnumerable<ListTipoProveedorDto>>> List(int id);
        Task<ResponseDto<SearchResultDto<SearchTipoProveedorDto>>> Search(SearchParamsDto<SearchTipoProveedorFilterDto> searchParams);
        Task<ResponseDto<IEnumerable<SelectComboTipoProveedorDto>>> SelectCombo();
        Task<ResponseDto<SearchResultDto<SelectTipoProveedorDto>>> Select(SearchParamsDto<SelectTipoProveedorFilterDto> searchParams);

    }
}

