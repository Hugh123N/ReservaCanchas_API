using System;
using System.Collections.Generic;

namespace Reserva.Dto.Dbo.Reserva
{
    public class ReservaDto
    {
        public Guid IdCliente { get; set; }
        public string? CodigoReserva { get; set; }
        public int IdCancha { get; set; }
        public int IdTipoDeporte { get; set; }
        public DateTimeOffset FechaReserva { get; set; }
        public decimal MontoTotal { get; set; }
        public int IdEstadoReserva { get; set; }
        public DateTimeOffset? FechaExpiracionPreReserva { get; set; }
        public int? IdOperadorConfirmo { get; set; }
        public DateTimeOffset? FechaConfirmacion { get; set; }
    }
}
