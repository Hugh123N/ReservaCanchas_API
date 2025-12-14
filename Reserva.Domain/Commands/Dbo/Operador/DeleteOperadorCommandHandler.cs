using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;
using Reserva.Domain.Commands.Dbo.Usuario;
using MediatR;

namespace Reserva.Domain.Commands.Dbo.Operador
{
    public class DeleteOperadorCommandHandler : CommandHandlerBase<DeleteOperadorCommand>
    {
        private readonly IRepository<Entity.Operador> _OperadorRepository;

        public DeleteOperadorCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            DeleteOperadorCommandValidator validator,
            IRepository<Entity.Operador> OperadorRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _OperadorRepository = OperadorRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteOperadorCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var Operador = await _OperadorRepository.GetByAsync(x => x.IdOperador == request.Id);

            var result = await _mediator!.Send(new DeleteUsuarioCommand(Operador!.IdUsuario), cancellationToken);

            if (result != null) {
                if (!result.IsValid)
                {
                    response.Messages = result.Messages;
                    return response;
                }
            }

            Operador.Activo = false;
            await _OperadorRepository.UpdateAsync(Operador);
            response.AddOkResult(Resources.Common.DeleteSuccessMessage);

            return response;
        }
    }
}
