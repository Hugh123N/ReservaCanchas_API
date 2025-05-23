namespace Reserva.Dto.Cancha.Disponibilidad
{
    public class GetDisponibilidadDto : DisponibilidadDto
    {
        public int IdDisponibilidad { get; set; }
        public bool Activo { get; set; }
    }
}
