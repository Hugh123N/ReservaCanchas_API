using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;
using Reserva.Domain.Services.GestionAdmin;
using Reserva.Dto.GestionAdmin;

namespace Reserva.Domain.Commands.Dbo.Proveedor
{
    public class DeleteProveedorCommandHandler : CommandHandlerBase<DeleteProveedorCommand>
    {
        private readonly IRepository<Entity.Proveedor> _ProveedorRepository;
        private readonly GestionAdminService _gestion;

        public DeleteProveedorCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteProveedorCommandValidator validator,
            IRepository<Entity.Proveedor> ProveedorRepository,
            GestionAdminService gestion
        ) : base(unitOfWork, mapper, validator)
        {
            _ProveedorRepository = ProveedorRepository;
            _gestion = gestion;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteProveedorCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var Proveedor = await _ProveedorRepository.GetByAsync(x => x.IdProveedor == request.Id);

            if (Proveedor != null)
            {
                return await _gestion.CambiarProveedorAsync(Proveedor.IdProveedor,
                    new CambiarEstadoGestionDto { Estado = NegocioReglas.Inactivo, Version = Proveedor.UpdateDate ?? Proveedor.CreateDate });
            }

            return response;
        }
    }
}
