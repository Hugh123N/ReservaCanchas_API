using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Dbo.Servicio;
using Reserva.Domain.Queries.Dbo.Servicio;
using Reserva.Dto.Dbo.Servicio;

namespace Reserva.Application.Dbo
{
    public class ServicioApplication : ApplicationBase, IServicioApplication
    {
        public ServicioApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetServicioDto>> Create(CreateServicioDto createDto)
            => await _mediator.Send(new CreateServicioCommand(createDto));
        public async Task<ResponseDto<GetServicioDto>> Update(UpdateServicioDto updateDto)
            => await _mediator.Send(new UpdateServicioCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeleteServicioCommand(id));
        public async Task<ResponseDto<GetServicioDto>> Get(int id)
            => await _mediator.Send(new GetServicioQuery(id));
        public async Task<ResponseDto<IEnumerable<SelectComboServicioDto>>> SelectCombo()
            => await _mediator.Send(new SelectComboServicioQuery());

    }
}
