namespace Reserva.Dto.Dbo.Operador
{
    public class CreateOperadorDto : OperadorDto
    {
        public string? Host { get; set; }
        public List<int>? CanchaIds { get; set; }
    }
}
