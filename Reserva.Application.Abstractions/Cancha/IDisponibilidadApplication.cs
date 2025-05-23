using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Disponibilidad;

namespace Reserva.Application.Abstractions.Cancha
{
    public interface IDisponibilidadApplication
    {
        Task<ResponseDto<GetDisponibilidadDto>> Create(CreateDisponibilidadDto createDto);
        Task<ResponseDto<GetDisponibilidadDto>> Update(UpdateDisponibilidadDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetDisponibilidadDto>> Get(int id);
        Task<ResponseDto<IEnumerable<ListDisponibilidadDto>>> List(int id);
        Task<ResponseDto<SearchResultDto<SearchDisponibilidadDto>>> Search(SearchParamsDto<SearchDisponibilidadFilterDto> searchParams);
        Task<ResponseDto<IEnumerable<SelectComboDisponibilidadDto>>> SelectCombo();
        Task<ResponseDto<SearchResultDto<SelectDisponibilidadDto>>> Select(SearchParamsDto<SelectDisponibilidadFilterDto> searchParams);

    }
}

