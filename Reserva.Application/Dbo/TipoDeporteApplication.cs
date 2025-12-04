using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Dbo.TipoDeporte;
using Reserva.Domain.Queries.Dbo.TipoDeporte;
using Reserva.Dto.Dbo.TipoDeporte;

namespace Reserva.Application.Dbo
{
    public class TipoDeporteApplication : ApplicationBase, ITipoDeporteApplication
    {
        public TipoDeporteApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetTipoDeporteDto>> Create(CreateTipoDeporteDto createDto)
            => await _mediator.Send(new CreateTipoDeporteCommand(createDto));
        public async Task<ResponseDto<GetTipoDeporteDto>> Update(UpdateTipoDeporteDto updateDto)
            => await _mediator.Send(new UpdateTipoDeporteCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeleteTipoDeporteCommand(id));
        public async Task<ResponseDto<GetTipoDeporteDto>> Get(int id)
            => await _mediator.Send(new GetTipoDeporteQuery(id));
        public async Task<ResponseDto<IEnumerable<SelectComboTipoDeporteDto>>> SelectCombo()
            => await _mediator.Send(new SelectComboTipoDeporteQuery());

    }
}
