using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Commands.Dbo.Usuario;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Cliente;
using Reserva.Dto.Dbo.Usuario;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.Cliente
{
    public class UpdateClienteCommandHandler : CommandHandlerBase<UpdateClienteCommand, GetClienteDto>
    {
        private readonly IRepository<Entity.Cliente> _ClienteRepository;
        private readonly UserManager<Entity.ApplicationUser> _UsuarioManager;

        public UpdateClienteCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            UpdateClienteCommandValidator validator,
            IRepository<Entity.Cliente> ClienteRepository,
            UserManager<Entity.ApplicationUser> userManager
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _ClienteRepository = ClienteRepository;
            _UsuarioManager = userManager;
        }

        public override async Task<ResponseDto<GetClienteDto>> HandleCommand(UpdateClienteCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetClienteDto>();

            var cliente = await _ClienteRepository.GetByAsync(x => x.IdCliente == request.UpdateDto.IdCliente && x.Activo);

            if (cliente == null)
            {
                response.AddErrorResult(Resources.Common.UpdateRecordNotFound);
                return response;
            }

            if (!Guid.TryParse(cliente.UserId, out var idUsuario))
            {
                response.AddErrorResult("El cliente no tiene un usuario asociado");
                return response;
            }

            var user = await _UsuarioManager.FindByIdAsync(cliente.UserId!);

            var userUpdateDto = new UpdateUsuarioDto
            {
                Id = idUsuario,
                // El cliente no edita su username: se conserva el existente (lo que devuelva el GET)
                UserName = user?.UserName,
                Email = request.UpdateDto.Email,
                PhoneNumber = request.UpdateDto.Telefono,
                FirstName = request.UpdateDto.Nombres,
                LastName = request.UpdateDto.Apellidos,
                Imagen = request.UpdateDto.Imagen ?? user?.Imagen
            };

            var result = await _mediator!.Send(new UpdateUsuarioCommand(userUpdateDto), cancellationToken);
            if (!result.IsValid)
            {
                response.Messages = result.Messages;
                return response;
            }

            var userId = cliente.UserId;
            _mapper?.Map(request.UpdateDto, cliente);
            // El vínculo con el usuario se establece solo al crear el cliente, nunca se modifica
            cliente.UserId = userId;

            await _ClienteRepository.UpdateAsync(cliente);

            var clienteDto = _mapper?.Map<GetClienteDto>(cliente);
            if (clienteDto != null) response.UpdateData(clienteDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
