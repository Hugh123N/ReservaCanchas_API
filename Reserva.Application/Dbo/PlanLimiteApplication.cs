using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Dbo.PlanLimite;
using Reserva.Domain.Queries.Dbo.PlanLimite;
using Reserva.Dto.Dbo.PlanLimite;

namespace Reserva.Application.Dbo
{
    public class PlanLimiteApplication : ApplicationBase, IPlanLimiteApplication
    {
        public PlanLimiteApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetPlanLimiteDto>> Create(CreatePlanLimiteDto createDto)
            => await _mediator.Send(new CreatePlanLimiteCommand(createDto));
        public async Task<ResponseDto<GetPlanLimiteDto>> Update(UpdatePlanLimiteDto updateDto)
            => await _mediator.Send(new UpdatePlanLimiteCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeletePlanLimiteCommand(id));
        public async Task<ResponseDto<GetPlanLimiteDto>> Get(int id)
            => await _mediator.Send(new GetPlanLimiteQuery(id));
        public async Task<ResponseDto<IEnumerable<ListPlanLimiteDto>>> List(int id)
            => await _mediator.Send(new ListPlanLimiteQuery(id));

    }
}
