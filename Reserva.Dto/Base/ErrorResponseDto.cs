namespace Reserva.Dto.Base
{
    public class ErrorResponseDto
    {
        public Guid Identifier { get; set; }
        public int StatusCode { get; set; }
        public string? Title { get; set; }
        public string? Message { get; set; }
        public string? StackTrace { get; set; }
    }
}
