using QRCoder;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Reserva.Domain.Services.Pago
{
    public class QrCodeService
    {
        /// <summary>
        /// Genera un QR code para Yape
        /// Formato: JSON con datos del pago
        /// </summary>
        public async Task<(string QrBase64, string QrText)> GenerarQrYape(string numeroTelefono, decimal monto, string concepto)
        {
            // Validar formato de teléfono peruano (9 dígitos que empieza con 9)
            if (!Regex.IsMatch(numeroTelefono, @"^9\d{8}$"))
            {
                throw new ArgumentException("El número de teléfono debe tener 9 dígitos y comenzar con 9", nameof(numeroTelefono));
            }

            // Crear objeto con datos del pago
            var datosPago = new
            {
                tipo = "YAPE",
                telefono = numeroTelefono,
                monto = monto.ToString("F2"),
                moneda = "PEN",
                concepto = concepto,
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            // Convertir a JSON
            string qrText = JsonSerializer.Serialize(datosPago, new JsonSerializerOptions
            {
                WriteIndented = false
            });

            // Generar QR
            string qrBase64 = await GenerarQrGenerico(qrText);

            return (qrBase64, qrText);
        }

        /// <summary>
        /// Genera un QR code para Plin
        /// Formato: JSON con datos del pago
        /// </summary>
        public async Task<(string QrBase64, string QrText)> GenerarQrPlin(string numeroTelefono, decimal monto, string concepto)
        {
            // Validar formato de teléfono peruano (9 dígitos que empieza con 9)
            if (!Regex.IsMatch(numeroTelefono, @"^9\d{8}$"))
            {
                throw new ArgumentException("El número de teléfono debe tener 9 dígitos y comenzar con 9", nameof(numeroTelefono));
            }

            // Crear objeto con datos del pago
            var datosPago = new
            {
                tipo = "PLIN",
                telefono = numeroTelefono,
                monto = monto.ToString("F2"),
                moneda = "PEN",
                concepto = concepto,
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            // Convertir a JSON
            string qrText = JsonSerializer.Serialize(datosPago, new JsonSerializerOptions
            {
                WriteIndented = false
            });

            // Generar QR
            string qrBase64 = await GenerarQrGenerico(qrText);

            return (qrBase64, qrText);
        }

        /// <summary>
        /// Genera un QR code genérico usando QRCoder
        /// NOTA: Requiere el paquete NuGet QRCoder
        /// </summary>
        public async Task<string> GenerarQrGenerico(string contenido)
        {
            return await Task.Run(() =>
            {
                try
                {

                    using var qrGenerator = new QRCodeGenerator();
                    using var qrCodeData = qrGenerator.CreateQrCode(contenido, QRCodeGenerator.ECCLevel.Q);
                    using var qrCode = new PngByteQRCode(qrCodeData);
                    byte[] qrCodeBytes = qrCode.GetGraphic(20);
                    return Convert.ToBase64String(qrCodeBytes);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Error al generar el código QR.", ex);
                }
            });
        }

        /// <summary>
        /// Valida el formato de un código de operación de Yape
        /// Formato esperado: Alfanumérico de 6-10 caracteres
        /// </summary>
        public bool ValidarCodigoOperacionYape(string codigoOperacion)
        {
            if (string.IsNullOrWhiteSpace(codigoOperacion))
                return false;

            // Yape típicamente usa códigos alfanuméricos de 6-10 caracteres
            return Regex.IsMatch(codigoOperacion, @"^[A-Z0-9]{6,10}$", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Valida el formato de un código de operación de Plin
        /// Formato esperado: Alfanumérico de 6-10 caracteres
        /// </summary>
        public bool ValidarCodigoOperacionPlin(string codigoOperacion)
        {
            if (string.IsNullOrWhiteSpace(codigoOperacion))
                return false;

            // Plin típicamente usa códigos alfanuméricos de 6-10 caracteres
            return Regex.IsMatch(codigoOperacion, @"^[A-Z0-9]{6,10}$", RegexOptions.IgnoreCase);
        }
    }
}
