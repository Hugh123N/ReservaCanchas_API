using System;

namespace Reserva.Dto.Dbo.ProveedorPlan
{
    public class PagoPlanDto
    {
        public int IdPagoPlan { get; set; }
        public int IdProveedorPlan { get; set; }
        public decimal Monto { get; set; }
        public string? Moneda { get; set; }
        public int IdMetodoPago { get; set; }
        public string? MetodoPagoNombre { get; set; }
        public int IdEstadoPago { get; set; }
        public string? EstadoPagoNombre { get; set; }
        public DateTimeOffset? FechaPago { get; set; }
        public string? CulqiChargeId { get; set; }
        public string? CodigoOperacion { get; set; }
        public bool Activo { get; set; }
    }
}