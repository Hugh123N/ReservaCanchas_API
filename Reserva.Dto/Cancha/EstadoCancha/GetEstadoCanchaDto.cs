namespace Reserva.Dto.Cancha.EstadoCancha
{
    public class GetEstadoCanchaDto : EstadoCanchaDto
    {
        public int IdEstadoCancha { get; set; }
        public bool Activo { get; set; }
    }
}
