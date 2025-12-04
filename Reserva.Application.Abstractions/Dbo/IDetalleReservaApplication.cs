using Reserva.Dto.Base;
using Reserva.Dto.Dbo.DetalleReserva;

namespace Reserva.Application.Abstractions.Dbo
{
    public interface IDetalleReservaApplication
    {
        Task<ResponseDto<GetDetalleReservaDto>> Create(CreateDetalleReservaDto createDto);
        Task<ResponseDto<GetDetalleReservaDto>> Update(UpdateDetalleReservaDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetDetalleReservaDto>> Get(int id);
        Task<ResponseDto<IEnumerable<ListDetalleReservaDto>>> List(int id);

    }
}

