using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.TipoProveedor
{
    public class DeleteTipoProveedorCommandHandler : CommandHandlerBase<DeleteTipoProveedorCommand>
    {
        private readonly IRepository<Entity.Models.TipoProveedor> _TipoProveedorRepository;

        public DeleteTipoProveedorCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteTipoProveedorCommandValidator validator,
            IRepository<Entity.Models.TipoProveedor> TipoProveedorRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _TipoProveedorRepository = TipoProveedorRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteTipoProveedorCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var TipoProveedor = await _TipoProveedorRepository.GetByAsync(x => x.IdTipoProveedor == request.Id);

            if (TipoProveedor != null)
            {
                TipoProveedor.Activo = false;
                await _TipoProveedorRepository.UpdateAsync(TipoProveedor);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
