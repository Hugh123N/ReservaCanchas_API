using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;
using Reserva.Domain.Commands.Dbo.Usuario;
using MediatR;

namespace Reserva.Domain.Commands.Dbo.Cliente
{
    public class DeleteClienteCommandHandler : CommandHandlerBase<DeleteClienteCommand>
    {
        private readonly IRepository<Entity.Cliente> _ClienteRepository;

        public DeleteClienteCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            DeleteClienteCommandValidator validator,
            IRepository<Entity.Cliente> ClienteRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _ClienteRepository = ClienteRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteClienteCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var Cliente = await _ClienteRepository.GetByAsync(x => x.IdCliente == request.Id);

            if (Cliente != null) {
                Cliente.Activo = false;
                await _ClienteRepository.UpdateAsync(Cliente);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
