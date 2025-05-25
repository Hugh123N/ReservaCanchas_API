using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Proveedor;

namespace Reserva.Application.Abstractions.Cancha
{
    public interface IProveedorApplication
    {
        Task<ResponseDto<GetProveedorDto>> Create(CreateProveedorDto createDto);
        Task<ResponseDto<GetProveedorDto>> Update(UpdateProveedorDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetProveedorDto>> Get(int id);
        Task<ResponseDto<IEnumerable<ListProveedorDto>>> List(int id);
        Task<ResponseDto<SearchResultDto<SearchProveedorDto>>> Search(SearchParamsDto<SearchProveedorFilterDto> searchParams);
        Task<ResponseDto<IEnumerable<SelectComboProveedorDto>>> SelectCombo();
        Task<ResponseDto<SearchResultDto<SelectProveedorDto>>> Select(SearchParamsDto<SelectProveedorFilterDto> searchParams);

    }
}

