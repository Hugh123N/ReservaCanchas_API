using AutoMapper;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Token;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Security.Cryptography;
//using Newtonsoft.Json;

namespace Reserva.Domain.Commands.Token
{
    public class GenerateTokenCommandHandler : CommandHandlerBase<GenerateTokenCommand, AccessTokenDto>
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<Entity.ApplicationUser> _userManager;

        public GenerateTokenCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            IConfiguration configuration,
            UserManager<Entity.ApplicationUser> userManager
        ) : base(unitOfWork, mapper, mediator)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public override async Task<ResponseDto<AccessTokenDto>> HandleCommand(GenerateTokenCommand request, CancellationToken cancellationToken)
        {
            var usuario = request.Usuario;

            var now = DateTime.UtcNow;
            var issuer = _configuration["SecurityOptions:Issuer"];
            var audience = _configuration["SecurityOptions:Audience"];
            var expiration = _configuration.GetValue<int>("SecurityOptions:ExpirationInSeconds");
            var securityKey = _configuration["SecurityOptions:SecurityKey"];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var roles = await _userManager.GetRolesAsync(request.Usuario);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Email ?? usuario.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, now.ToString(), ClaimValueTypes.String),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email ?? ""),
                new Claim("UserId", usuario.Id.ToString()),
                new Claim("UserIdNegocio", request.idUsuarioNegocio.ToString() ?? ""),
                new Claim("DisplayName", $"{usuario.UserName} {usuario.LastName}" ?? ""),
                new Claim("UserName", usuario.UserName ?? ""),
                new Claim("Telefono", usuario.PhoneNumber ?? "")
            };
            if(roles != null && roles.Any())
            {
                foreach (var role in roles)
                    claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwt = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: now,
                expires: now.AddSeconds(expiration),
                signingCredentials: signingCredentials
            );

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var accessTokenDto = new AccessTokenDto
            {
                access_token = encodedJwt,
                expires_in = expiration
            };

            return await Task.FromResult(new ResponseDto<AccessTokenDto>(accessTokenDto));
        }

        public static string EncriptarSHA256(string text)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));

                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                    builder.Append(b.ToString("x2"));

            return builder.ToString();
                
            }
        }
    }
}
