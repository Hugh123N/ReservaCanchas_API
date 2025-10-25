namespace Reserva.Dto.Dbo.EstadoReserva
{
    public class GetEstadoReservaDto : EstadoReservaDto
    {
        public int IdEstadoReserva { get; set; }
        public bool Activo { get; set; }
    }
}
