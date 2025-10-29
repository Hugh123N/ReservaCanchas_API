namespace Reserva.Dto.Dbo.Reserva
{
    public class CreateReservaDto : ReservaDto
    {
        public string CodigoMetodoPago { get; set; } = null!;

        /// <summary>
        /// Monto del adelanto (solo para Efectivo)
        /// Si es 0 o null, se crea sin adelanto
        /// </summary>
        public decimal? MontoAdelanto { get; set; }
    }
}
