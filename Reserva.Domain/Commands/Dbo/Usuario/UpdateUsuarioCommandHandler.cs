using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Usuario;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.Usuario
{
    public class UpdateUsuarioCommandHandler : CommandHandlerBase<UpdateUsuarioCommand, GetUsuarioDto>
    {
        private readonly IRepository<Entity.AspNetUsers> _UsuarioRepository;
        private readonly UserManager<Entity.ApplicationUser> _userManager;

        public UpdateUsuarioCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateUsuarioCommandValidator validator,
            IRepository<Entity.AspNetUsers> UsuarioRepository,
            UserManager<Entity.ApplicationUser> userManager
        ) : base(unitOfWork, mapper, validator)
        {
            _UsuarioRepository = UsuarioRepository;
            _userManager = userManager;
        }

        public override async Task<ResponseDto<GetUsuarioDto>> HandleCommand(UpdateUsuarioCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetUsuarioDto>();

            var usuario = await _UsuarioRepository.GetByAsync(x => x.Id == request.UpdateDto.Id);

            if (usuario == null)
            {
                response.AddErrorResult(Resources.Common.UpdateRecordNotFound);
                return response;
            }

            var newUserName = request.UpdateDto.UserName?.Trim();

            if (!string.IsNullOrWhiteSpace(newUserName) &&
                !string.Equals(usuario.UserName, newUserName, StringComparison.OrdinalIgnoreCase))
            {
                // Validar unicidad
                var existing = await _userManager.FindByNameAsync(newUserName);

                if (existing != null && existing.Id != usuario.Id)
                {
                    response.AddErrorResult("El nombre de usuario ya está en uso.");
                    return response;
                }

                usuario.NormalizedUserName =
                    _userManager.NormalizeName(newUserName);
            }

            _mapper?.Map(request.UpdateDto, usuario);

            // 4. Guardar los cambios
            await _UsuarioRepository.UpdateAsync(usuario);

            var UsuarioDto = _mapper?.Map<GetUsuarioDto>(usuario);
            if (UsuarioDto != null) response.UpdateData(UsuarioDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
