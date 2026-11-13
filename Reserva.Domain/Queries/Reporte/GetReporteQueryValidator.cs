using FluentValidation;
using Reserva.Common;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Reporte;

namespace Reserva.Domain.Queries.Reporte;

public class GetReporteQueryValidator : QueryValidatorBase<GetReporteQuery>
{
    public GetReporteQueryValidator()
    {
        RuleFor(q => q.Filtro.FechaDesde).NotEqual(default(DateOnly)).WithMessage("Selecciona la fecha inicial.");
        RuleFor(q => q.Filtro.FechaHasta).NotEqual(default(DateOnly)).LessThan(DateOnly.MaxValue);
        RuleFor(q => q.Filtro).Must(f => f.FechaHasta >= f.FechaDesde && f.FechaHasta.DayNumber - f.FechaDesde.DayNumber <= 365)
            .WithMessage("El rango debe ser válido y no superar 366 días.");
        RuleFor(q => q.Filtro.IdCancha).GreaterThan(0).When(q => q.Filtro.IdCancha.HasValue);
        RuleFor(q => q.Filtro.EstadoReserva).Must(e => string.IsNullOrEmpty(e) || e == "VIGENTES" ||
            e == Constants.ESTADO_RESERVA.Pendiente || e == Constants.ESTADO_RESERVA.Confirmado ||
            e == Constants.ESTADO_RESERVA.Cancelado || e == Constants.ESTADO_RESERVA.Expirado).WithMessage("Estado de reserva inválido.");
        RuleFor(q => q.Filtro.EstadoPago).Must(e => string.IsNullOrEmpty(e) ||
            e == Constants.ESTADO_PAGO.Pendiente || e == Constants.ESTADO_PAGO.Parcial || e == Constants.ESTADO_PAGO.Pagado ||
            e == Constants.ESTADO_PAGO.Cancelado || e == Constants.ESTADO_PAGO.Rechazado).WithMessage("Estado de pago inválido.");
        When(q => q.Tipo == TipoReporte.AgendaOperador, () => RuleFor(q => q.Filtro)
            .Must(f => f.FechaDesde == f.FechaHasta).WithMessage("La agenda corresponde a un solo día."));
        When(q => q.Tipo == TipoReporte.Demanda, () => RuleFor(q => q.Filtro)
            .Must(f => string.IsNullOrEmpty(f.EstadoReserva) && string.IsNullOrEmpty(f.EstadoPago))
            .WithMessage("Demanda compara los estados por separado; no admite filtros de estado."));
    }
}
