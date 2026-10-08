using AutoMapper;
using MediatR;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Commands.Dbo.Usuario;
using Reserva.Dto.Dbo.Proveedor;
using Reserva.Dto.Dbo.Usuario;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.Proveedor
{
    public class UpdateProveedorCommandHandler : CommandHandlerBase<UpdateProveedorCommand, GetProveedorDto>
    {
        private readonly IRepository<Entity.Proveedor> _ProveedorRepository;

        public UpdateProveedorCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            UpdateProveedorCommandValidator validator,
            IRepository<Entity.Proveedor> ProveedorRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _ProveedorRepository = ProveedorRepository;
        }

        public override async Task<ResponseDto<GetProveedorDto>> HandleCommand(UpdateProveedorCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetProveedorDto>();
            var Proveedor = await _ProveedorRepository.GetByAsync(x => x.IdProveedor == request.UpdateDto.IdProveedor);

            if (Proveedor == null)
            {
                response.AddErrorResult(Resources.Common.UpdateRecordNotFound);
                return response;
            }

            // El usuario asociado (AspNetUsers) se actualiza dentro de la misma transacción
            if (!Guid.TryParse(Proveedor.IdUsuario, out var idUsuario))
            {
                response.AddErrorResult("El proveedor no tiene un usuario asociado");
                return response;
            }

            var userUpdateDto = new UpdateUsuarioDto
            {
                Id = idUsuario,
                UserName = request.UpdateDto.Email,
                Email = request.UpdateDto.Email,
                PhoneNumber = request.UpdateDto.Telefono,
                FirstName = request.UpdateDto.Nombres,
                LastName = request.UpdateDto.Apellidos,
                Imagen = request.UpdateDto.Imagen
            };

            var result = await _mediator!.Send(new UpdateUsuarioCommand(userUpdateDto), cancellationToken);
            if (!result.IsValid)
            {
                response.Messages = result.Messages;
                return response;
            }

            _mapper?.Map(request.UpdateDto, Proveedor);
            await _ProveedorRepository.UpdateAsync(Proveedor);
            // NO llamar SaveAsync() - UnitOfWork hace commit automático al final

            var ProveedorDto = _mapper?.Map<GetProveedorDto>(Proveedor);
            if (ProveedorDto != null) response.UpdateData(ProveedorDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return response;
        }
    }
}
