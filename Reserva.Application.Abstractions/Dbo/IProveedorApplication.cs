using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Proveedor;

namespace Reserva.Application.Abstractions.Dbo
{
    public interface IProveedorApplication
    {
        Task<ResponseDto<GetProveedorDto>> Create(CreateProveedorDto createDto);
        Task<ResponseDto<GetProveedorDto>> Update(UpdateProveedorDto updateDto);
        Task<ResponseDto> Delete(Guid id);
        Task<ResponseDto<GetProveedorDto>> Get(Guid id);
        Task<ResponseDto<IEnumerable<ListProveedorDto>>> List(Guid id);
        Task<ResponseDto<SearchResultDto<SearchProveedorDto>>> Search(SearchParamsDto<SearchProveedorFilterDto> searchParams);
        Task<ResponseDto<IEnumerable<SelectComboProveedorDto>>> SelectCombo();
        Task<ResponseDto<SearchResultDto<SelectProveedorDto>>> Select(SearchParamsDto<SelectProveedorFilterDto> searchParams);

    }
}

