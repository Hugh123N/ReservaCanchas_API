using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Dbo.Plane;
using Reserva.Domain.Queries.Dbo.Plane;
using Reserva.Dto.Dbo.Plane;

namespace Reserva.Application.Dbo
{
    public class PlaneApplication : ApplicationBase, IPlaneApplication
    {
        public PlaneApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetPlaneDto>> Create(CreatePlaneDto createDto)
            => await _mediator.Send(new CreatePlaneCommand(createDto));
        public async Task<ResponseDto<GetPlaneDto>> Update(UpdatePlaneDto updateDto)
            => await _mediator.Send(new UpdatePlaneCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeletePlaneCommand(id));
        public async Task<ResponseDto<GetPlaneDto>> Get(int id)
            => await _mediator.Send(new GetPlaneQuery(id));
        public async Task<ResponseDto<IEnumerable<ListPlaneDto>>> List()
            => await _mediator.Send(new ListPlaneQuery());

    }
}
