namespace Reserva.Domain.Services.WhatsApp
{
    /// <summary>
    /// Servicio para enviar mensajes por WhatsApp Cloud API
    /// </summary>
    public interface IWhatsAppService
    {
        /// <summary>
        /// Envía un mensaje de texto simple a un número de WhatsApp
        /// </summary>
        /// <param name="phoneNumber">Número de teléfono con código de país (ej: +51987654321)</param>
        /// <param name="message">Mensaje de texto a enviar</param>
        /// <returns>True si el mensaje se envió exitosamente</returns>
        Task<bool> SendTextMessageAsync(string phoneNumber, string message);

        /// <summary>
        /// Envía un mensaje de texto a múltiples números de WhatsApp
        /// </summary>
        /// <param name="phoneNumbers">Lista de números con código de país</param>
        /// <param name="message">Mensaje de texto a enviar</param>
        /// <returns>Número de mensajes enviados exitosamente</returns>
        Task<int> SendBulkTextMessageAsync(List<string> phoneNumbers, string message);
    }
}
