namespace Reserva.Dto.Dbo.DetalleReserva
{
    public class GetDetalleReservaDto : DetalleReservaDto
    {
        public int IdDetalleReserva { get; set; }
        public bool Activo { get; set; }
    }
}
