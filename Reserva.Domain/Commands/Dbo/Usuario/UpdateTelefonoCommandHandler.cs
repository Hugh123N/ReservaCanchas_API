using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.Usuario
{
    public class UpdateTelefonoCommandHandler : CommandHandlerBase<UpdateTelefonoCommand>
    {
        private readonly IRepository<Entity.AspNetUsers> _UsuarioRepository;

        public UpdateTelefonoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IRepository<Entity.AspNetUsers> UsuarioRepository
        ) : base(unitOfWork, mapper)
        {
            _UsuarioRepository = UsuarioRepository;
        }

        public override async Task<ResponseDto> HandleCommand(UpdateTelefonoCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var usuario = await _UsuarioRepository.GetByAsync(x => x.Id == request.IdUsuario);

            if (usuario == null)
            {
                response.AddErrorResult("Usuario no encontrado");
                return response;
            }

            usuario.PhoneNumber = request.Telefono;
            await _UsuarioRepository.UpdateAsync(usuario);
            await _UsuarioRepository.SaveAsync();

            response.AddOkResult("Teléfono actualizado correctamente");

            return await Task.FromResult(response);
        }
    }
}
