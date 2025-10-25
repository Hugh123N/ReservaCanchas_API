using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.Usuario
{
    public class DeleteUsuarioCommandHandler : CommandHandlerBase<DeleteUsuarioCommand>
    {
        private readonly IRepository<Entity.AspNetUsers> _UsuarioRepository;

        public DeleteUsuarioCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteUsuarioCommandValidator validator,
            IRepository<Entity.AspNetUsers> UsuarioRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _UsuarioRepository = UsuarioRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteUsuarioCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var Usuario = await _UsuarioRepository.GetByAsync(x => x.Id == request.Id);

            if (Usuario != null)
            {
                Usuario.Activo = false;
                await _UsuarioRepository.UpdateAsync(Usuario);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
