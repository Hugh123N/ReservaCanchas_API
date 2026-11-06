using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Commands.Token;
using Reserva.Domain.Queries.Dbo.Usuario;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Token;
using Reserva.Repository.Abstractions.Transactions;
using Reserva.Repository.Security;

namespace Reserva.Domain.Commands.User
{
    public class RenewSessionCommandHandler : CommandHandlerBase<RenewSessionCommand, AccessTokenDto>
    {
        private readonly IUserIdentity _userIdentity;
        private readonly UserManager<Entity.ApplicationUser> _userManager;

        public RenewSessionCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            IUserIdentity userIdentity,
            UserManager<Entity.ApplicationUser> userManager
        ) : base(unitOfWork, mapper, mediator)
        {
            _userIdentity = userIdentity;
            _userManager = userManager;
        }


        public override async Task<ResponseDto<AccessTokenDto>> HandleCommand(RenewSessionCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<AccessTokenDto>();
            var userId = _userIdentity.GetCurrentUserId();
            if (!userId.HasValue)
            {
                response.AddErrorResult("La sesión no es válida. Inicia sesión nuevamente.");
                return response;
            }

            var user = await _userManager.FindByIdAsync(userId.Value.ToString());
            if (user == null || await _userManager.IsLockedOutAsync(user))
            {
                response.AddErrorResult("No se pudo renovar la sesión del usuario.");
                return response;
            }

            var applicationCode = _userIdentity.GetCurrentUserClaims()
                .FirstOrDefault(claim => claim.Type == "ApplicationCode")?.Value?.Trim() ?? string.Empty;
            var roles = await _userManager.GetRolesAsync(user);
            var negocio = await _mediator!.Send(new GetUsuarioNegocioQuery(user.Id, applicationCode, roles), cancellationToken);
            if (!negocio.IsValid)
            {
                response.AttachResults(negocio);
                return response;
            }

            var accessToken = await _mediator!.Send(
                new GenerateTokenCommand(applicationCode, user, negocio.Data), cancellationToken);

            if (!accessToken.IsValid || string.IsNullOrWhiteSpace(accessToken.Data?.access_token))
            {
                response.AttachResults(accessToken);
                response.AddErrorResult("No se pudo generar el token de renovación.");
                return response;
            }

            return accessToken;
        }
    }
}
