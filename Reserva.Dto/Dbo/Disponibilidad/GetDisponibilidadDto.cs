namespace Reserva.Dto.Dbo.Disponibilidad
{
    public class GetDisponibilidadDto : DisponibilidadDto
    {
        public int IdDisponibilidad { get; set; }
        public bool Activo { get; set; }
    }
}
