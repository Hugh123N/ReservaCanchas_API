namespace Reserva.Dto.Cancha.EstadoProveedor
{
    public class GetEstadoProveedorDto : EstadoProveedorDto
    {
        public int IdEstadoProveedor { get; set; }
        public bool Activo { get; set; }
    }
}
