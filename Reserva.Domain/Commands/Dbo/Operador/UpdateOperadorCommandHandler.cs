using AutoMapper;
using MediatR;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Commands.Dbo.Usuario;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Operador;
using Reserva.Dto.Dbo.Usuario;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.Operador
{
    public class UpdateOperadorCommandHandler : CommandHandlerBase<UpdateOperadorCommand, GetOperadorDto>
    {
        private readonly IRepository<Entity.Operador> _OperadorRepository;

        public UpdateOperadorCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            UpdateOperadorCommandValidator validator,
            IRepository<Entity.Operador> OperadorRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _OperadorRepository = OperadorRepository;
        }

        public override async Task<ResponseDto<GetOperadorDto>> HandleCommand(UpdateOperadorCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetOperadorDto>();

            var operador = await _OperadorRepository.GetByAsync(x => x.IdOperador == request.UpdateDto.IdOperador && x.Activo,
                x => x.OperadorCancha);

            var userUpdateDto = new UpdateUsuarioDto
            {
                Id = operador.IdUsuario,
                UserName = request.UpdateDto.Email,
                Email = request.UpdateDto.Email,
                PhoneNumber = request.UpdateDto.Telefono,
                FirstName = request.UpdateDto.Nombre,
                LastName = request.UpdateDto.Apellidos
            };

            var result = await _mediator!.Send(new UpdateUsuarioCommand(userUpdateDto), cancellationToken);
            if (!result.IsValid)
            {
                response.Messages = result.Messages;
                return response;
            }

            if (request.UpdateDto.CanchaIds != null)
            {
                operador.OperadorCancha.Where(x => !request.UpdateDto.CanchaIds.Contains(x.IdCancha))
                    .ToList().ForEach(x => x.Activo = false);

                foreach (var idCancha in request.UpdateDto.CanchaIds)
                {
                    var existente = operador.OperadorCancha.FirstOrDefault(x => x.IdCancha == idCancha);
                    if (existente != null)
                    {
                        existente.Activo = true;
                    }
                    else
                    {
                        operador.OperadorCancha.Add(new Entity.OperadorCancha
                        {
                            IdCancha = idCancha,
                            IdOperador = operador.IdOperador,
                            Activo = true
                        });
                    }
                }
            }

            await _OperadorRepository.UpdateAsync(operador);
            await _OperadorRepository.SaveAsync();

            var OperadorDto = _mapper?.Map<GetOperadorDto>(operador);
            if (OperadorDto != null) response.UpdateData(OperadorDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
