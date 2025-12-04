using Reserva.Dto.Base;
using Reserva.Dto.Dbo.TipoSuperficie;

namespace Reserva.Application.Abstractions.Dbo
{
    public interface ITipoSuperficieApplication
    {
        Task<ResponseDto<GetTipoSuperficieDto>> Create(CreateTipoSuperficieDto createDto);
        Task<ResponseDto<GetTipoSuperficieDto>> Update(UpdateTipoSuperficieDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetTipoSuperficieDto>> Get(int id);
        Task<ResponseDto<IEnumerable<SelectComboTipoSuperficieDto>>> SelectCombo();

    }
}

