using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Dbo.DetalleReserva;
using Reserva.Domain.Queries.Dbo.DetalleReserva;
using Reserva.Dto.Dbo.DetalleReserva;

namespace Reserva.Application.Dbo
{
    public class DetalleReservaApplication : ApplicationBase, IDetalleReservaApplication
    {
        public DetalleReservaApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetDetalleReservaDto>> Create(CreateDetalleReservaDto createDto)
            => await _mediator.Send(new CreateDetalleReservaCommand(createDto));
        public async Task<ResponseDto<GetDetalleReservaDto>> Update(UpdateDetalleReservaDto updateDto)
            => await _mediator.Send(new UpdateDetalleReservaCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeleteDetalleReservaCommand(id));
        public async Task<ResponseDto<GetDetalleReservaDto>> Get(int id)
            => await _mediator.Send(new GetDetalleReservaQuery(id));
        public async Task<ResponseDto<IEnumerable<ListDetalleReservaDto>>> List(int id)
            => await _mediator.Send(new ListDetalleReservaQuery(id));

    }
}
