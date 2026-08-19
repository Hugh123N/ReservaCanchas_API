using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Proveedor;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;
using Reserva.Dto.Dbo.Usuario;
using Reserva.Domain.Commands.Dbo.Usuario;

namespace Reserva.Domain.Commands.Dbo.Proveedor
{
    public class CreateProveedorCommandHandler : CommandHandlerBase<CreateProveedorCommand, GetProveedorDto>
    {
        private readonly IRepository<Entity.Proveedor> _ProveedorRepository;
        private readonly IRepository<Entity.EstadoProveedor> _EstadoProveedorRepository;
        private readonly IRepository<Entity.TipoProveedor> _TipoProveedorRepository;

        public CreateProveedorCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateProveedorCommandValidator validator,
            IRepository<Entity.Proveedor> ProveedorRepository,
            IRepository<Entity.EstadoProveedor> EstadoProveedorRepository,
            IRepository<Entity.TipoProveedor> TipoProveedorRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _ProveedorRepository = ProveedorRepository;
            _EstadoProveedorRepository = EstadoProveedorRepository;
            _TipoProveedorRepository = TipoProveedorRepository;
        }

        public override async Task<ResponseDto<GetProveedorDto>> HandleCommand(CreateProveedorCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetProveedorDto>();
            var proveedorDto = request.CreateDto;

            var estadoProveedor = await _EstadoProveedorRepository.GetByAsNoTrackingAsync(x => x.Codigo.Equals(Constants.ESTADO_PROVEEDOR.Aprobado));
            var tipoProveedor = await _TipoProveedorRepository.GetByAsNoTrackingAsync(x => x.Codigo.Equals(Constants.TIPO_PROVEEDOR.persona_natural));

            var Proveedor = _mapper?.Map<Entity.Proveedor>(proveedorDto);

            if (Proveedor != null)
            {
                var user = new CreateUsuarioDto
                {
                    UserName = proveedorDto.UserName,
                    Email = proveedorDto.Email,
                    Password = proveedorDto.Password,
                    ConfirmPassword = proveedorDto.ConfirmPassword,
                    PhoneNumber = proveedorDto.Telefono,
                    FirstName = proveedorDto.Nombre,
                    LastName = proveedorDto.Apellidos,
                    RoleIds = new List<Guid>{Guid.Parse(Constants.RoleIds.Proveedor)}
                };

                var responseUser = await _mediator!.Send(new CreateUsuarioCommand(user));
                if(responseUser.Data == null || responseUser!.IsValid == false)
                {
                    response.Messages = responseUser.Messages;
                    return response;
                }

                Proveedor.IdTipoProveedor = tipoProveedor!.IdTipoProveedor;
                Proveedor.IdEstadoProveedor = estadoProveedor!.IdEstadoProveedor;
                Proveedor.IdUsuario = responseUser.Data.Id;

                await _ProveedorRepository.AddAsync(Proveedor);
                await _ProveedorRepository.SaveAsync();
            }

            var ProveedorDto = _mapper?.Map<GetProveedorDto>(Proveedor);
            if (ProveedorDto != null) response.UpdateData(ProveedorDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}