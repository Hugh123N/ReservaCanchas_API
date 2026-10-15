using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Dbo.Cliente;
using Reserva.Domain.Queries.Dbo.Cliente;
using Reserva.Dto.Dbo.Cliente;

namespace Reserva.Application.Dbo
{
    public class ClienteApplication : ApplicationBase, IClienteApplication
    {
        public ClienteApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetClienteDto>> Create(CreateClienteRegistroDto createDto)
            => await _mediator.Send(new CreateClienteRegistroCommand(createDto));
        public async Task<ResponseDto<GetClienteDto>> Update(UpdateClienteDto updateDto)
            => await _mediator.Send(new UpdateClienteCommand(updateDto));
        public async Task<ResponseDto<GetClienteDto>> Get(int id)
            => await _mediator.Send(new GetClienteQuery(id));
    }
}
