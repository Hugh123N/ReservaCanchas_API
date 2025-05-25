using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.EstadoUsuario
{
    public class DeleteEstadoUsuarioCommandHandler : CommandHandlerBase<DeleteEstadoUsuarioCommand>
    {
        private readonly IRepository<Entity.Models.EstadoUsuario> _EstadoUsuarioRepository;

        public DeleteEstadoUsuarioCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteEstadoUsuarioCommandValidator validator,
            IRepository<Entity.Models.EstadoUsuario> EstadoUsuarioRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _EstadoUsuarioRepository = EstadoUsuarioRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteEstadoUsuarioCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var EstadoUsuario = await _EstadoUsuarioRepository.GetByAsync(x => x.IdEstadoUsuario == request.Id);

            if (EstadoUsuario != null)
            {
                EstadoUsuario.Activo = false;
                await _EstadoUsuarioRepository.UpdateAsync(EstadoUsuario);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
