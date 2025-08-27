using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Org.BouncyCastle.Crypto.Generators;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Base;
using Reserva.Entity;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Domain.Commands.Cancha.Usuario
{
    internal class ResetPasswordCommandHandler : CommandHandlerBase<ResetPasswordCommand>
    {
        private readonly UserManager<ApplicationUser> _userRepository;

        public ResetPasswordCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            //ResetPasswordCommandValidator validator,
            UserManager<ApplicationUser> userRepository
        ) : base(unitOfWork, mapper, mediator)
        {
            _userRepository = userRepository;
        }

        public override async Task<ResponseDto> HandleCommand(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            
            var user = await _userRepository.FindByEmailAsync(request.ResetPasswordDto.Email);
            if (user == null)
            {
                response.AddErrorResult("Usuario no encontrado.");
                return response;
            }
            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.ResetPasswordDto.Token));

            var result = await _userRepository.ResetPasswordAsync(user,token, request.ResetPasswordDto.NewPassword);

            if (!result.Succeeded) {
                foreach (var error in result.Errors)
                {
                    response.AddErrorResult(error.Description);
                }
                return response;
            }

            response.AddOkResult("Contraseña actualizada correctamente.");
            return response;

        }
    }
}
