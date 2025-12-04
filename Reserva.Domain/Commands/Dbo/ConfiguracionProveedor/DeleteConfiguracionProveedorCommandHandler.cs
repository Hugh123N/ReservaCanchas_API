using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.ConfiguracionProveedor
{
    public class DeleteConfiguracionProveedorCommandHandler : CommandHandlerBase<DeleteConfiguracionProveedorCommand>
    {
        private readonly IRepository<Entity.ConfiguracionProveedor> _ConfiguracionProveedorRepository;

        public DeleteConfiguracionProveedorCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteConfiguracionProveedorCommandValidator validator,
            IRepository<Entity.ConfiguracionProveedor> ConfiguracionProveedorRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _ConfiguracionProveedorRepository = ConfiguracionProveedorRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteConfiguracionProveedorCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var ConfiguracionProveedor = await _ConfiguracionProveedorRepository.GetByAsync(x => x.IdConfiguracionProveedor == request.Id);

            if (ConfiguracionProveedor != null)
            {
                ConfiguracionProveedor.Activo = false;
                await _ConfiguracionProveedorRepository.UpdateAsync(ConfiguracionProveedor);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
