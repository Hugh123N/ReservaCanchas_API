using AutoMapper;
using MediatR;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Commands.Dbo.Usuario;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Cliente;
using Reserva.Dto.Dbo.Usuario;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.Cliente
{
    public class CreateClienteRegistroCommandHandler : CommandHandlerBase<CreateClienteRegistroCommand, GetClienteDto>
    {
        public CreateClienteRegistroCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateClienteRegistroCommandValidator validator
        ) : base(unitOfWork, mapper, mediator, validator)
        {
        }

        public override async Task<ResponseDto<GetClienteDto>> HandleCommand(CreateClienteRegistroCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetClienteDto>();

            var createUserDto = new CreateUsuarioDto
            {
                // El registro público usa el correo como nombre de usuario cuando no viene informado
                UserName = string.IsNullOrWhiteSpace(request.CreateDto.UserName)
                    ? request.CreateDto.Email
                    : request.CreateDto.UserName,
                Email = request.CreateDto.Email,
                PhoneNumber = request.CreateDto.PhoneNumber,
                FirstName = request.CreateDto.FirstName,
                LastName = request.CreateDto.LastName,
                Password = request.CreateDto.Password,
                ConfirmPassword = request.CreateDto.ConfirmPassword,
                RoleIds = new List<Guid> { Guid.Parse(Constants.RoleIds.Cliente) }
            };

            var resultUser = await _mediator!.Send(new CreateUsuarioCommand(createUserDto), cancellationToken);
            if (!resultUser.IsValid)
            {
                response.Messages = resultUser.Messages;
                return response;
            }

            var createClienteDto = new CreateClienteDto
            {
                Nombres = request.CreateDto.FirstName,
                Apellidos = request.CreateDto.LastName,
                Telefono = request.CreateDto.PhoneNumber,
                Email = request.CreateDto.Email,
                UserId = resultUser.Data!.Id.ToString()
            };

            var resultCliente = await _mediator!.Send(new CreateClienteCommand(createClienteDto), cancellationToken);
            if (!resultCliente.IsValid)
            {
                response.Messages = resultCliente.Messages;
                return response;
            }

            if (resultCliente.Data != null) response.UpdateData(resultCliente.Data);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
