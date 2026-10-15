using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Cliente;

namespace Reserva.Application.Abstractions.Dbo
{
    public interface IClienteApplication
    {
        Task<ResponseDto<GetClienteDto>> Create(CreateClienteRegistroDto createDto);
        Task<ResponseDto<GetClienteDto>> Update(UpdateClienteDto updateDto);
        Task<ResponseDto<GetClienteDto>> Get(int id);
    }
}
