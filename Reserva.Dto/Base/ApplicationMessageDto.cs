namespace Reserva.Dto.Base
{
    public class ApplicationMessageDto
    {
        public string? Key { get; set; }
        public string? Message { get; set; }
        public ApplicationMessageType MessageType { get; set; }
    }
}
