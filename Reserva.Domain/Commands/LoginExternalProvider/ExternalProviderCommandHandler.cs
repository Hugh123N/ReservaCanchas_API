using Google.Apis.Auth;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Base;
using Reserva.Entity;
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
        public ExternalProviderCommandHandler(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public override async Task<ResponseDto<Entity.ApplicationUser>> HandleCommand(ExternalProviderCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<Entity.ApplicationUser>();
            var nuevoUsuario = new Entity.ApplicationUser();
            try
            {
                if (request.CreateDto.TypeValidation.Contains(Constants.TIPO_VALIDACION.GOOGLE))
                {
                    try
                    {
                        var payload = await GoogleJsonWebSignature.ValidateAsync(
                            request.CreateDto.IdToken,
                            new GoogleJsonWebSignature.ValidationSettings
                            {
                                Audience = new[] { "133409574804-gucuitfeqfj0lkceuaa1qcukcfoib8ib.apps.googleusercontent.com" } // obligatorio en producción
                            }
                        );

                        if (string.IsNullOrEmpty(payload.Email))
                        {
                            response.AddErrorResult("No se recibió un correo válido desde Google.");
                            return response;
                        }

                        nuevoUsuario = new Entity.ApplicationUser
                        {
                            Email = payload.Email,
                            UserName = payload.Email,
                            FirstName = payload.GivenName,
                            LastName = payload.FamilyName
                        };
                    }
                    catch (InvalidJwtException ex)
                    {
                        response.AddErrorResult("El token de Google no es válido: " + ex.Message);
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.AddErrorResult("Error inesperado en autenticación con Google.");
                        return response;
                    }

                }
                else if (request.CreateDto.TypeValidation.Contains(Constants.TIPO_VALIDACION.FACEBOOK)) {
                    // Validación de token de Facebook (simplificada)
                    var appAccessToken =";{app-id}|{app-secret}"; // Reemplaza con tus valores
                    var fbTokenValidationUrl = $"https://graph.facebook.com/debug_token?input_token={request.CreateDto.IdToken}&access_token={appAccessToken}";
                    using (var httpClient = new HttpClient())
                    {
                        var fbResponse = await httpClient.GetStringAsync(fbTokenValidationUrl);
                        dynamic fbData = Newtonsoft.Json.JsonConvert.DeserializeObject(fbResponse);
                        if (fbData.data.is_valid != true)
                        {
                            response.AddErrorResult("El token de Facebook no es válido.");
                            return response;
                        }
                        // Obtener información del usuario desde Facebook
                        var userInfoUrl = $"https://graph.facebook.com/me?fields=id,name,email,first_name,last_name&access_token={request.CreateDto.IdToken}";
                        var userInfoResponse = await httpClient.GetStringAsync(userInfoUrl);
                        dynamic userInfo = Newtonsoft.Json.JsonConvert.DeserializeObject(userInfoResponse);
                        if (userInfo.email == null)
                        {
                            response.AddErrorResult("No se recibió un correo válido desde Facebook.");
                            return response;
                        }
                        nuevoUsuario = new Entity.ApplicationUser
                        {
                            Email = userInfo.email,
                            UserName = userInfo.email,
                            FirstName = userInfo.first_name,
                            LastName = userInfo.last_name
                        };
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
