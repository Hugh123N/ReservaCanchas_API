namespace Reserva.Dto.Dbo.DetalleReserva
{
    public class GetDetalleReservaDto : DetalleReservaDto
    {
        public int IdDetalleReserva { get; set; }
        public TimeOnly? HoraInicio { get; set; }
        public TimeOnly? HoraFin { get; set; }
        public decimal? Precio { get; set; }
    }
}
