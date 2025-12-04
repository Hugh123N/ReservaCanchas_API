using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.ConfiguracionProveedor;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.ConfiguracionProveedor
{
    public class CreateConfiguracionProveedorCommandHandler : CommandHandlerBase<CreateConfiguracionProveedorCommand, GetConfiguracionProveedorDto>
    {
        private readonly IRepository<Entity.ConfiguracionProveedor> _ConfiguracionProveedorRepository;

        public CreateConfiguracionProveedorCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateConfiguracionProveedorCommandValidator validator,
            IRepository<Entity.ConfiguracionProveedor> ConfiguracionProveedorRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _ConfiguracionProveedorRepository = ConfiguracionProveedorRepository;
        }

        public override async Task<ResponseDto<GetConfiguracionProveedorDto>> HandleCommand(CreateConfiguracionProveedorCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetConfiguracionProveedorDto>();

            var ConfiguracionProveedor = _mapper?.Map<Entity.ConfiguracionProveedor>(request.CreateDto);

            if (ConfiguracionProveedor != null)
            {
                await _ConfiguracionProveedorRepository.AddAsync(ConfiguracionProveedor);
                await _ConfiguracionProveedorRepository.SaveAsync();
            }

            var ConfiguracionProveedorDto = _mapper?.Map<GetConfiguracionProveedorDto>(ConfiguracionProveedor);
            if (ConfiguracionProveedorDto != null) response.UpdateData(ConfiguracionProveedorDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}