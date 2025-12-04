namespace Reserva.Dto.Dbo.Servicio
{
    public class GetServicioDto : ServicioDto
    {
        public int IdServicio { get; set; }
        public bool Activo { get; set; }
    }
}
