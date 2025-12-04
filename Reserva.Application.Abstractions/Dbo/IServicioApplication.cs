using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Servicio;

namespace Reserva.Application.Abstractions.Dbo
{
    public interface IServicioApplication
    {
        Task<ResponseDto<GetServicioDto>> Create(CreateServicioDto createDto);
        Task<ResponseDto<GetServicioDto>> Update(UpdateServicioDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetServicioDto>> Get(int id);
        Task<ResponseDto<IEnumerable<SelectComboServicioDto>>> SelectCombo();

    }
}

