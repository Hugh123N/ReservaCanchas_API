using Reserva.Dto.Base;
using Reserva.Dto.Cancha.EstadoProveedor;

namespace Reserva.Application.Abstractions.Cancha
{
    public interface IEstadoProveedorApplication
    {
        Task<ResponseDto<GetEstadoProveedorDto>> Create(CreateEstadoProveedorDto createDto);
        Task<ResponseDto<GetEstadoProveedorDto>> Update(UpdateEstadoProveedorDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetEstadoProveedorDto>> Get(int id);
        Task<ResponseDto<IEnumerable<ListEstadoProveedorDto>>> List(int id);
        Task<ResponseDto<SearchResultDto<SearchEstadoProveedorDto>>> Search(SearchParamsDto<SearchEstadoProveedorFilterDto> searchParams);
        Task<ResponseDto<IEnumerable<SelectComboEstadoProveedorDto>>> SelectCombo();
        Task<ResponseDto<SearchResultDto<SelectEstadoProveedorDto>>> Select(SearchParamsDto<SelectEstadoProveedorFilterDto> searchParams);

    }
}

