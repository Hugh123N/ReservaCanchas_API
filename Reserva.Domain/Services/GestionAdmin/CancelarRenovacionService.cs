using Reserva.Domain.Services.Culqi;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Services.GestionAdmin;

public class CancelarRenovacionService(ICulqiService culqi, IRepository<Entity.ProveedorPlan> planes)
{
    public async Task<ResponseDto> CancelarAsync(IEnumerable<Entity.ProveedorPlan> suscripciones, string motivo)
    {
        var response = new ResponseDto();
        var seleccion = suscripciones.Where(p => !p.CancelAtPeriodEnd && (p.AutoRenovacion ||
            !string.IsNullOrWhiteSpace(p.CulqiSubscriptionId) && p.Estado != Reserva.Common.Constants.ESTADO_PROV_PLAN.CANCELLED) ||
            !string.IsNullOrWhiteSpace(p.CulqiSubscriptionIdAnterior)).ToList();
        var ids = seleccion.SelectMany(p => new[] { p.CulqiSubscriptionId, p.CulqiSubscriptionIdAnterior })
            .Where(id => !string.IsNullOrWhiteSpace(id)).Distinct().ToList();
        // Las cancelaciones externas no son transaccionales: deben admitir reintentos si falla una parte del lote.
        foreach (var id in ids)
        {
            if (!await culqi.CancelSubscriptionAsync(id!))
            {
                response.AddErrorResult("No se pudo confirmar la cancelación de todas las renovaciones en la pasarela. No se aplicó el cambio local del proveedor. Vuelve a intentar; las cancelaciones ya confirmadas no se reactivan.");
                return response;
            }
        }
        foreach (var plan in seleccion)
        {
            plan.AutoRenovacion = false;
            plan.CancelAtPeriodEnd = true;
            plan.FechaProximoCobro = null;
            plan.MotivoCancelacion = motivo;
            plan.CulqiSubscriptionIdAnterior = null;
            await planes.UpdateAsync(plan);
        }
        return response;
    }
}
