using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Domain.Extensions
{
    public class JwtHelper
    {
        public string DecodeJwtAndReadClaims(string authorizationHeader)
        {
            // Verificamos si el header Authorization es válido (debe tener el formato "Bearer <token>")
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("El header Authorization no contiene un token JWT válido.");
            }

            // Extraer el token JWT (el texto después de "Bearer ")
            var token = authorizationHeader.Substring("Bearer ".Length).Trim();

            try
            {
                // Crear un manejador para procesar el JWT
                var handler = new JwtSecurityTokenHandler();

                // Decodificar el token JWT (esto no valida la firma, solo lo decodifica)
                var jsonToken = handler.ReadJwtToken(token);

                // Leer los claims del token
                var claims = jsonToken.Claims.ToList();

                // Mostrar los claims
                foreach (var claim in claims)
                {
                    Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
                }

                // Ejemplo de cómo leer claims específicos
                var username = claims.FirstOrDefault(c => c.Type == "sub")?.Value;  // sub es comúnmente el ID de usuario
                var role = claims.FirstOrDefault(c => c.Type == "role")?.Value;

                Console.WriteLine($"Username (sub): {username}");
                Console.WriteLine($"Role: {role}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al decodificar el token JWT: {ex.Message}");
            }

            return "";
        }
    }
}
