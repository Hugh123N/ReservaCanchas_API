using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.EstadoProveedor
{
    public class DeleteEstadoProveedorCommandHandler : CommandHandlerBase<DeleteEstadoProveedorCommand>
    {
        private readonly IRepository<Entity.Models.EstadoProveedor> _EstadoProveedorRepository;

        public DeleteEstadoProveedorCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteEstadoProveedorCommandValidator validator,
            IRepository<Entity.Models.EstadoProveedor> EstadoProveedorRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _EstadoProveedorRepository = EstadoProveedorRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteEstadoProveedorCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var EstadoProveedor = await _EstadoProveedorRepository.GetByAsync(x => x.IdEstadoProveedor == request.Id);

            if (EstadoProveedor != null)
            {
                EstadoProveedor.Activo = false;
                await _EstadoProveedorRepository.UpdateAsync(EstadoProveedor);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
