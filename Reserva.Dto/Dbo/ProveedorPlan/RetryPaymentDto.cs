namespace Reserva.Dto.Dbo.ProveedorPlan
{
    public class RetryPaymentDto
    {
        public int IdProveedorPlan { get; set; }
        // CulqiToken no es necesario para suscripciones - Culqi usa la tarjeta guardada
    }
}