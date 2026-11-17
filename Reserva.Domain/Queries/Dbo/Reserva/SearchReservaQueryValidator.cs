using FluentValidation;
using Reserva.Common;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Reserva;

namespace Reserva.Domain.Queries.Dbo.Reserva;

public class SearchReservaQueryValidator : SearchQueryValidatorBase<SearchReservaQuery, SearchReservaFilterDto, ReservaClienteDto, SearchReservasResultDto>
{
    public SearchReservaQueryValidator()
    {
        When(q => q.SearchParams?.Page != null, () => RuleFor(q => q.SearchParams.Page!.PageSize)
            .LessThanOrEqualTo(200).WithMessage("La página no puede superar 200 reservas."));
        When(q => q.SearchParams?.Page != null, () => RuleFor(q => q.SearchParams.Page!)
            .Must(p => (long)(p.Page - 1) * p.PageSize <= int.MaxValue).WithMessage("Número de página fuera de rango."));
        When(q => q.SearchParams?.Filter != null, () =>
        {
            RuleFor(q => q.SearchParams.Filter!.SearchText).MaximumLength(150);
            RuleFor(q => q.SearchParams.Filter!.NombreCancha).MaximumLength(150);
            RuleFor(q => q.SearchParams.Filter!.IdCancha).GreaterThan(0).When(q => q.SearchParams.Filter!.IdCancha.HasValue);
            RuleFor(q => q.SearchParams.Filter!).Must(f => !Desde(f).HasValue || !Hasta(f).HasValue || Desde(f) <= Hasta(f))
                .WithMessage("La fecha inicial no puede ser posterior a la final.");
            RuleFor(q => q.SearchParams.Filter!).Must(f => !Hasta(f).HasValue || Hasta(f) < DateOnly.MaxValue)
                .WithMessage("Fecha final no válida.");
            RuleFor(q => q.SearchParams.Filter!).Must(f => !Desde(f).HasValue || Desde(f) > DateOnly.MinValue)
                .WithMessage("Fecha inicial no válida.");
            RuleFor(q => q.SearchParams.Filter!.CodigoEstado).Must(e => string.IsNullOrEmpty(e) ||
                e is "VIGENTES" or "ANULADAS" || e == Constants.ESTADO_RESERVA.Pendiente || e == Constants.ESTADO_RESERVA.Confirmado ||
                e == Constants.ESTADO_RESERVA.Cancelado || e == Constants.ESTADO_RESERVA.Expirado || e == Constants.ESTADO_RESERVA.NoPresentado || e == Constants.ESTADO_RESERVA.Completado)
                .WithMessage("Estado de reserva inválido.");
            RuleFor(q => q.SearchParams.Filter!.CodigoEstadoPago).Must(e => string.IsNullOrEmpty(e) || e == Constants.ESTADO_PAGO.Pendiente ||
                e == Constants.ESTADO_PAGO.Parcial || e == Constants.ESTADO_PAGO.Pagado || e == Constants.ESTADO_PAGO.Cancelado || e == Constants.ESTADO_PAGO.Rechazado)
                .WithMessage("Estado de pago inválido.");
        });
    }

    public static DateOnly? Desde(SearchReservaFilterDto f) => f.FechaDesdeLocal ?? (f.FechaDesde.HasValue ? DateOnly.FromDateTime(f.FechaDesde.Value.DateTime) : null);
    public static DateOnly? Hasta(SearchReservaFilterDto f) => f.FechaHastaLocal ?? (f.FechaHasta.HasValue ? DateOnly.FromDateTime(f.FechaHasta.Value.DateTime) : null);
}
