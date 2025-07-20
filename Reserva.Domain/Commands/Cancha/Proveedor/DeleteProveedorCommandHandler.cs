using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Proveedor
{
    public class DeleteProveedorCommandHandler : CommandHandlerBase<DeleteProveedorCommand>
    {
        private readonly IRepository<Entity.Models.Proveedor> _ProveedorRepository;

        public DeleteProveedorCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteProveedorCommandValidator validator,
            IRepository<Entity.Models.Proveedor> ProveedorRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _ProveedorRepository = ProveedorRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteProveedorCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var Proveedor = await _ProveedorRepository.GetByAsync(x => x.IdProveedor == request.Id);

            if (Proveedor != null)
            {
                Proveedor.Activo = false;
                await _ProveedorRepository.UpdateAsync(Proveedor);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
