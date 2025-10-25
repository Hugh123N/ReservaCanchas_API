using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Dbo.Notificacion;
using Reserva.Domain.Queries.Dbo.Notificacion;
using Reserva.Dto.Dbo.Notificacion;

namespace Reserva.Application.Dbo
{
    public class NotificacionApplication : ApplicationBase, INotificacionApplication
    {
        public NotificacionApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetNotificacionDto>> Create(CreateNotificacionDto createDto)
            => await _mediator.Send(new CreateNotificacionCommand(createDto));
        public async Task<ResponseDto<GetNotificacionDto>> Update(UpdateNotificacionDto updateDto)
            => await _mediator.Send(new UpdateNotificacionCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeleteNotificacionCommand(id));
        public async Task<ResponseDto<GetNotificacionDto>> Get(int id)
            => await _mediator.Send(new GetNotificacionQuery(id));
        public async Task<ResponseDto<IEnumerable<ListNotificacionDto>>> List(int id)
            => await _mediator.Send(new ListNotificacionQuery(id));
        public async Task<ResponseDto<SearchResultDto<SearchNotificacionDto>>> Search(SearchParamsDto<SearchNotificacionFilterDto> searchParams)
            => await _mediator.Send(new SearchNotificacionQuery(searchParams));
        public async Task<ResponseDto<IEnumerable<SelectComboNotificacionDto>>> SelectCombo()
            => await _mediator.Send(new SelectComboNotificacionQuery());
        public async Task<ResponseDto<SearchResultDto<SelectNotificacionDto>>> Select(SearchParamsDto<SelectNotificacionFilterDto> searchParams)
             => await _mediator.Send(new SelectNotificacionQuery(searchParams));

    }
}
