using MediatR;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Dbo.Reclamacion;
using Reserva.Domain.Queries.Dbo.Reclamacion;
using Reserva.Domain.Services.Notificacion;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Reclamacion;

namespace Reserva.Application.Dbo
{
    public class ReclamacionApplication : ApplicationBase, IReclamacionApplication
    {
        private readonly INotificacionService _notificacionService;

        public ReclamacionApplication(IMediator mediator, INotificacionService notificacionService) : base(mediator)
        {
            _notificacionService = notificacionService;
        }

        public async Task<ResponseDto<GetReclamacionDto>> Create(CreateReclamacionDto createDto, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new CreateReclamacionCommand(createDto), cancellationToken);
            if (!response.IsValid || response.Data == null) return response;

            // La notificación se realiza después del commit para conservar la hoja ante un fallo de correo.
            response.Data.CopiaEnviada = await _notificacionService.NotificarReclamacionRegistradaAsync(response.Data);
            if (response.Data.CopiaEnviada == false)
                response.AddWarningResult("La hoja se registró, pero no se pudo enviar la copia. Imprime o guarda la constancia mostrada.");

            return response;
        }

        public async Task<ResponseDto<GetReclamacionDto>> Get(int id, CancellationToken cancellationToken)
            => await _mediator.Send(new GetReclamacionQuery(id), cancellationToken);

        public async Task<ResponseDto<SearchResultDto<SearchReclamacionDto>>> Search(
            SearchParamsDto<SearchReclamacionFilterDto> searchParams, CancellationToken cancellationToken)
            => await _mediator.Send(new SearchReclamacionQuery(searchParams), cancellationToken);

        public async Task<ResponseDto> Atender(int id, AtenderReclamacionDto atenderDto, CancellationToken cancellationToken)
            => await _mediator.Send(new AtenderReclamacionCommand(id, atenderDto), cancellationToken);
    }
}
