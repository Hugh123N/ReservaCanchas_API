namespace Reserva.Dto.Cancha.Reserva
{
    public class GetReservaDto : ReservaDto
    {
        public int IdReserva { get; set; }
        public bool Activo { get; set; }
    }
}
