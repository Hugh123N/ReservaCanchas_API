namespace Reserva.Dto.Dbo.Reserva
{
    public class CreateReservaDto : ReservaDto
    {
        public List<CreateReservaDetalleDto> Detalles { get; set; } = new List<CreateReservaDetalleDto>();
        public string CodigoMetodoPago { get; set; } = null!;

        /// <summary>
        /// Monto del adelanto (solo para Efectivo)
        /// Si es 0 o null, se crea sin adelanto
        /// </summary>
        public decimal? MontoAdelanto { get; set; }
    }

    public class CreateReservaDetalleDto
    {
        public TimeOnly HoraInicio { get; set; }
        public TimeOnly HoraFin { get; set; }
    }
}
