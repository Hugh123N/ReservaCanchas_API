using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Dbo.TipoSuperficie;
using Reserva.Domain.Queries.Dbo.TipoSuperficie;
using Reserva.Dto.Dbo.TipoSuperficie;

namespace Reserva.Application.Dbo
{
    public class TipoSuperficieApplication : ApplicationBase, ITipoSuperficieApplication
    {
        public TipoSuperficieApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetTipoSuperficieDto>> Create(CreateTipoSuperficieDto createDto)
            => await _mediator.Send(new CreateTipoSuperficieCommand(createDto));
        public async Task<ResponseDto<GetTipoSuperficieDto>> Update(UpdateTipoSuperficieDto updateDto)
            => await _mediator.Send(new UpdateTipoSuperficieCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeleteTipoSuperficieCommand(id));
        public async Task<ResponseDto<GetTipoSuperficieDto>> Get(int id)
            => await _mediator.Send(new GetTipoSuperficieQuery(id));
        public async Task<ResponseDto<IEnumerable<SelectComboTipoSuperficieDto>>> SelectCombo()
            => await _mediator.Send(new SelectComboTipoSuperficieQuery());

    }
}
