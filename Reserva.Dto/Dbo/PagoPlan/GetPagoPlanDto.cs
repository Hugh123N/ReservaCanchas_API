namespace Reserva.Dto.Dbo.PagoPlan
{
    public class GetPagoPlanDto : PagoPlanDto
    {
        public int IdPagoPlan { get; set; }
        public string EstadoPago { get; set; } = null!;
    }
}
