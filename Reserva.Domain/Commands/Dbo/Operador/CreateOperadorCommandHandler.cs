using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Commands.Dbo.Usuario;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Operador;
using Reserva.Dto.Dbo.Usuario;
using Reserva.Entity;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.Operador
{
    public class CreateOperadorCommandHandler : CommandHandlerBase<CreateOperadorCommand, GetOperadorDto>
    {
        private readonly IRepository<Entity.Operador> _OperadorRepository;
        private readonly IRepository<Entity.OperadorCancha> _OperadorCanchaRepository;

        public CreateOperadorCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateOperadorCommandValidator validator,
            IRepository<Entity.Operador> OperadorRepository,
            IRepository<Entity.OperadorCancha> OperadorCanchaRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _OperadorRepository = OperadorRepository;
            _OperadorCanchaRepository = OperadorCanchaRepository;
        }

        public override async Task<ResponseDto<GetOperadorDto>> HandleCommand(CreateOperadorCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetOperadorDto>();

            var createUserDto = new CreateUsuarioDto
            { 
                UserName = request.CreateDto.Email,
                Email = request.CreateDto.Email,
                PhoneNumber = request.CreateDto.Telefono,
                FirstName = request.CreateDto.Nombre,
                LastName = request.CreateDto.Apellidos,
                Host = request.CreateDto.Host,
                RoleIds = new List<Guid> { Guid.Parse(Constants.RoleIds.Operador) }
            };

            var resultUser = await _mediator!.Send(new CreateUsuarioCommand(createUserDto), cancellationToken);

            if (!resultUser.IsValid) {
                response.Messages = resultUser.Messages;
                return response;
            }

            var operadorCanchas = new List<Entity.OperadorCancha>();
            if (request.CreateDto.CanchaIds != null)
            {
                operadorCanchas = request.CreateDto.CanchaIds?.Select(idCancha => new Entity.OperadorCancha
                {
                    IdCancha = idCancha
                }).ToList();
            }

            var Operador = new Entity.Operador
            {
                IdUsuario= resultUser.Data!.Id,
                IdProveedor = request.CreateDto.IdProveedor,
                OperadorCancha = operadorCanchas
            };

            await _OperadorRepository.AddAsync(Operador);
            await _OperadorRepository.SaveAsync();

            var OperadorDto = _mapper?.Map<GetOperadorDto>(Operador);
            if (OperadorDto != null) response.UpdateData(OperadorDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}