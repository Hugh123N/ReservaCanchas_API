namespace Reserva.Dto.Dbo.Calendario
{
    public class CrearReservaOperadorRequestDto
    {
        /// <summary>
        /// Tipo de reserva (Normal = 1, Inmediata = 2)
        /// </summary>
        public TipoReservaOperador TipoReserva { get; set; }

        public int IdCancha { get; set; }

        public int IdTipoDeporte { get; set; }

        public ClienteReservaDto Cliente { get; set; } = null!;

        public List<BloqueHorarioDto> Horarios { get; set; } = new List<BloqueHorarioDto>();

        public PagoReservaOperadorDto Pago { get; set; } = null!;

        public string? Observaciones { get; set; }
    }
}
