using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Reserva.Common;
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

namespace Reserva.Domain.Commands.LoginExternalProvider
{
    public class ExternalProviderCommandHandler : CommandHandlerBase<ExternalProviderCommand, Entity.ApplicationUser>
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ExternalProviderCommandHandler> _logger;
        private readonly IRepository<Entity.EstadoUsuario> _EstadoUsuarioRepository;

        public ExternalProviderCommandHandler(
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IRepository<Entity.EstadoUsuario> EstadoUsuarioRepository,
        ILogger<ExternalProviderCommandHandler> logger) : base(unitOfWork)
        {
            _configuration = configuration;
            _logger = logger;
            _EstadoUsuarioRepository = EstadoUsuarioRepository;
        }

        public override async Task<ResponseDto<Entity.ApplicationUser>> HandleCommand(ExternalProviderCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<Entity.ApplicationUser>();
            var nuevoUsuario = new Entity.ApplicationUser();
            try
            {
                var estadoActivo = await _EstadoUsuarioRepository.GetByAsNoTrackingAsync(e => e.Codigo == Constants.ESTADO_USUARIO.Activo);

                if (request.CreateDto.TypeValidation.Contains(Constants.TIPO_VALIDACION.GOOGLE))
                {
                    var googleClientId = _configuration["OAuth:Google:ClientId"];

                    if (string.IsNullOrEmpty(googleClientId))
                    {
                        _logger.LogError("Google ClientId no está configurado en appsettings.json");
                        response.AddErrorResult("Configuración de Google OAuth no disponible.");
                        return response;
                    }

                    try
                    {
                        _logger.LogInformation("Validando token de Google para autenticación");

                        var payload = await GoogleJsonWebSignature.ValidateAsync(
                            request.CreateDto.IdToken,
                            new GoogleJsonWebSignature.ValidationSettings
                            {
                                Audience = new[] { googleClientId }
                            }
                        );

                        if (string.IsNullOrEmpty(payload.Email))
                        {
                            _logger.LogWarning("Token de Google válido pero sin email");
                            response.AddErrorResult("No se recibió un correo válido desde Google.");
                            return response;
                        }

                        _logger.LogInformation("Usuario autenticado con Google: {Email}", payload.Email);

                        nuevoUsuario = new Entity.ApplicationUser
                        {
                            Email = payload.Email,
                            UserName = payload.Email,
                            FirstName = payload.GivenName ?? "",
                            LastName = payload.FamilyName ?? "",
                            IdEstadoUsuario = estadoActivo!.IdEstadoUsuario

                        };
                    }
                    catch (InvalidJwtException ex)
                    {
                        _logger.LogError(ex, "Token de Google inválido");
                        response.AddErrorResult("El token de Google no es válido: " + ex.Message);
                        return response;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error inesperado en autenticación con Google");
                        response.AddErrorResult("Error inesperado en autenticación con Google.");
                        return response;
                    }

                }
                else if (request.CreateDto.TypeValidation.Contains(Constants.TIPO_VALIDACION.FACEBOOK))
                {
                    var facebookAppId = _configuration["OAuth:Facebook:AppId"];
                    var facebookAppSecret = _configuration["OAuth:Facebook:AppSecret"];

                    if (string.IsNullOrEmpty(facebookAppId) || string.IsNullOrEmpty(facebookAppSecret))
                    {
                        _logger.LogError("Facebook AppId o AppSecret no están configurados en appsettings.json");
                        response.AddErrorResult("Configuración de Facebook OAuth no disponible.");
                        return response;
                    }

                    try
                    {
                        _logger.LogInformation("Validando token de Facebook para autenticación");

                        // App Access Token para validar el token del usuario
                        var appAccessToken = $"{facebookAppId}|{facebookAppSecret}";
                        var fbTokenValidationUrl = $"https://graph.facebook.com/debug_token?input_token={request.CreateDto.IdToken}&access_token={appAccessToken}";

                        using (var httpClient = new HttpClient())
                        {
                            // Validar el token
                            var fbResponse = await httpClient.GetStringAsync(fbTokenValidationUrl);
                            dynamic fbData = Newtonsoft.Json.JsonConvert.DeserializeObject(fbResponse);

                            if (fbData?.data?.is_valid != true)
                            {
                                _logger.LogWarning("Token de Facebook inválido");
                                response.AddErrorResult("El token de Facebook no es válido.");
                                return response;
                            }

                            _logger.LogInformation("Token de Facebook válido, obteniendo información del usuario");

                            // Obtener información del usuario desde Facebook
                            var userInfoUrl = $"https://graph.facebook.com/me?fields=id,name,email,first_name,last_name&access_token={request.CreateDto.IdToken}";
                            var userInfoResponse = await httpClient.GetStringAsync(userInfoUrl);
                            dynamic userInfo = Newtonsoft.Json.JsonConvert.DeserializeObject(userInfoResponse);

                            if (userInfo?.email == null)
                            {
                                _logger.LogWarning("Token de Facebook válido pero sin email");
                                response.AddErrorResult("No se recibió un correo válido desde Facebook. Asegúrate de conceder permisos de email.");
                                return response;
                            }

                            _logger.LogInformation("Usuario autenticado con Facebook: {Email}", (string)userInfo.email);

                            nuevoUsuario = new Entity.ApplicationUser
                            {
                                Email = userInfo.email,
                                UserName = userInfo.email,
                                FirstName = userInfo.first_name ?? "",
                                LastName = userInfo.last_name ?? "",
                                IdEstadoUsuario = estadoActivo!.IdEstadoUsuario,
                            };
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                        _logger.LogError(ex, "Error de red al comunicarse con Facebook API");
                        response.AddErrorResult("Error al comunicarse con Facebook. Intenta nuevamente.");
                        return response;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error inesperado en autenticación con Facebook");
                        response.AddErrorResult("Error inesperado en autenticación con Facebook.");
                        return response;
                    }
                }
                else
                {
                    response.AddErrorResult("Proveedor externo no soportado.");
                    return response;
                }
            }
            catch
            {
                response.AddErrorResult("Token inválido o expirado.");
                return response;
            }

            response.Data = nuevoUsuario;
            return response;
        }
    }
}
