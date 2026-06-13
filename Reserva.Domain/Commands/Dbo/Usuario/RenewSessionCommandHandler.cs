using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Commands.Token;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Token;
using Reserva.Dto.User;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;
using Reserva.Repository.Security;
using System.Runtime.Intrinsics.X86;

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
            var userId = _userIdentity.GetCurrentUserId().ToString();
            var applicationCode = "CODE";//_userIdentity.GetApplicationCode();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return response;

            var accessToken = await _mediator?.Send(new GenerateTokenCommand(applicationCode, user, null), cancellationToken)!;

            if (accessToken.Data != null)
                response.UpdateData(accessToken.Data);

            return response;
        }
    }
}
