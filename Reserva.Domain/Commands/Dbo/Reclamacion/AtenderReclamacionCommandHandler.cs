using AutoMapper;
using MediatR;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Services.Reclamaciones;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;
using Reserva.Repository.Security;

namespace Reserva.Domain.Commands.Dbo.Reclamacion
{
    public class AtenderReclamacionCommandHandler : CommandHandlerBase<AtenderReclamacionCommand>
    {
        private readonly IRepository<Entity.Reclamacion> _ReclamacionRepository;
        private readonly IUserIdentity _userIdentity;

        public AtenderReclamacionCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            AtenderReclamacionCommandValidator validator,
            IRepository<Entity.Reclamacion> ReclamacionRepository,
            IUserIdentity userIdentity
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _ReclamacionRepository = ReclamacionRepository;
            _userIdentity = userIdentity;
        }

        public override async Task<ResponseDto> HandleCommand(AtenderReclamacionCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            if (!ReclamacionReglas.EsAdmin(_userIdentity))
            {
                response.AddErrorResult("Acceso restringido al administrador.");
                return response;
            }

            var reclamacion = await _ReclamacionRepository.GetByAsync(x => x.IdReclamacion == request.Id && x.Activo);
            if (reclamacion == null)
            {
                response.AddErrorResult("Hoja no encontrada.");
                return response;
            }

            if (reclamacion.Estado == "RESPONDIDA")
            {
                response.AddErrorResult("La respuesta registrada ya está cerrada y no se puede sobrescribir.");
                return response;
            }

            var atenderDto = request.AtenderDto;
            if (atenderDto.Estado == "RESPONDIDA" && atenderDto.FechaComunicacionRespuesta < reclamacion.FechaRegistro)
            {
                response.AddErrorResult("La comunicación no puede ser anterior al registro de la hoja.");
                return response;
            }

            reclamacion.Estado = atenderDto.Estado;
            reclamacion.Observaciones = ReclamacionReglas.Opcional(atenderDto.Observaciones);
            if (atenderDto.Estado == "RESPONDIDA")
            {
                reclamacion.Respuesta = atenderDto.Respuesta!.Trim();
                reclamacion.FechaComunicacionRespuesta = atenderDto.FechaComunicacionRespuesta;
                reclamacion.ReferenciaRespuesta = atenderDto.ReferenciaRespuesta!.Trim();
                reclamacion.ResponsableRespuesta = _userIdentity.GetCurrentUser();
            }

            await _ReclamacionRepository.UpdateAsync(reclamacion);
            response.AddOkResult("Atención registrada.");
            return response;
        }
    }
}
