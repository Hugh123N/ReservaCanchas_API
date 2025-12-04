using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.ConfiguracionProveedor;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.ConfiguracionProveedor
{
    public class UpdateConfiguracionProveedorCommandHandler : CommandHandlerBase<UpdateConfiguracionProveedorCommand, GetConfiguracionProveedorDto>
    {
        private readonly IRepository<Entity.ConfiguracionProveedor> _ConfiguracionProveedorRepository;

        public UpdateConfiguracionProveedorCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateConfiguracionProveedorCommandValidator validator,
            IRepository<Entity.ConfiguracionProveedor> ConfiguracionProveedorRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _ConfiguracionProveedorRepository = ConfiguracionProveedorRepository;
        }

        public override async Task<ResponseDto<GetConfiguracionProveedorDto>> HandleCommand(UpdateConfiguracionProveedorCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetConfiguracionProveedorDto>();
            var ConfiguracionProveedor = await _ConfiguracionProveedorRepository.GetByAsync(x => x.IdConfiguracionProveedor == request.UpdateDto.IdConfiguracionProveedor);

            if (ConfiguracionProveedor != null)
            {
                _mapper?.Map(request.UpdateDto, ConfiguracionProveedor);
                await _ConfiguracionProveedorRepository.UpdateAsync(ConfiguracionProveedor);
                await _ConfiguracionProveedorRepository.SaveAsync();
            }

            var ConfiguracionProveedorDto = _mapper?.Map<GetConfiguracionProveedorDto>(ConfiguracionProveedor);
            if (ConfiguracionProveedorDto != null) response.UpdateData(ConfiguracionProveedorDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
