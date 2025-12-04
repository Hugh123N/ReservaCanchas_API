using Reserva.Dto.Base;
using Reserva.Dto.Dbo.ConfiguracionProveedor;

namespace Reserva.Application.Abstractions.Dbo
{
    public interface IConfiguracionProveedorApplication
    {
        Task<ResponseDto<GetConfiguracionProveedorDto>> Create(CreateConfiguracionProveedorDto createDto);
        Task<ResponseDto<GetConfiguracionProveedorDto>> Update(UpdateConfiguracionProveedorDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetConfiguracionProveedorDto>> Get(int id);
        Task<ResponseDto<IEnumerable<ListConfiguracionProveedorDto>>> List(int id);
        Task<ResponseDto<SearchResultDto<SearchConfiguracionProveedorDto>>> Search(SearchParamsDto<SearchConfiguracionProveedorFilterDto> searchParams);
        Task<ResponseDto<IEnumerable<SelectComboConfiguracionProveedorDto>>> SelectCombo();

    }
}

