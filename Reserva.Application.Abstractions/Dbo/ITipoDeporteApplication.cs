using Reserva.Dto.Base;
using Reserva.Dto.Dbo.TipoDeporte;

namespace Reserva.Application.Abstractions.Dbo
{
    public interface ITipoDeporteApplication
    {
        Task<ResponseDto<GetTipoDeporteDto>> Create(CreateTipoDeporteDto createDto);
        Task<ResponseDto<GetTipoDeporteDto>> Update(UpdateTipoDeporteDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetTipoDeporteDto>> Get(int id);
        Task<ResponseDto<IEnumerable<SelectComboTipoDeporteDto>>> SelectCombo();

    }
}

