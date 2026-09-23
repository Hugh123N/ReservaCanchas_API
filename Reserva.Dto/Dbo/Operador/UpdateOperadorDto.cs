namespace Reserva.Dto.Dbo.Operador
{
    public class UpdateOperadorDto : OperadorDto
    {
        public int IdOperador { get; set; }
        public List<int>? CanchaIds { get; set; }
    }
}
