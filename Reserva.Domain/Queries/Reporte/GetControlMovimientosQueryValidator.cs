using FluentValidation;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Reporte;

namespace Reserva.Domain.Queries.Reporte;

public class GetControlMovimientosQueryValidator : QueryValidatorBase<GetControlMovimientosQuery>
{
    public GetControlMovimientosQueryValidator(GetReporteQueryValidator periodos)
    {
        RuleFor(q => q.Filtro).NotNull().DependentRules(() =>
        {
            RuleFor(q => q.Filtro.Movimientos).NotNull().DependentRules(() =>
            {
                RuleFor(q => q.Filtro.Movimientos).Custom((f, context) =>
                {
                    var resultado = periodos.Validate(new GetReporteQuery(TipoReporte.Economico,
                        new FiltroReporteDto { FechaDesde = f.FechaDesde, FechaHasta = f.FechaHasta, IdCancha = f.IdCancha }));
                    foreach (var error in resultado.Errors) context.AddFailure(error.PropertyName, error.ErrorMessage);
                });
                RuleFor(q => q.Filtro.Movimientos.IdUsuarioResponsable).NotEqual(Guid.Empty)
                    .When(q => q.Filtro.Movimientos.IdUsuarioResponsable.HasValue);
                RuleFor(q => q.Filtro.Movimientos).Must(f => !(f.SoloSinResponsable && f.IdUsuarioResponsable.HasValue))
                    .WithMessage("Selecciona una persona o históricos sin identificar, no ambos.");
                RuleFor(q => q.Filtro.Movimientos.IdMetodoPago).GreaterThan(0).When(q => q.Filtro.Movimientos.IdMetodoPago.HasValue);
                RuleFor(q => q.Filtro.Movimientos.Tipo).Must(t => string.IsNullOrEmpty(t) || t is "COBRO" or "REEMBOLSO" or "CORRECCION")
                    .WithMessage("Tipo de movimiento inválido.");
                RuleFor(q => q.Filtro.Movimientos).Must(f => f.Tipo != "CORRECCION" || !f.IdMetodoPago.HasValue)
                    .WithMessage("Una corrección global no tiene método de cobro atribuible.");
                RuleFor(q => q.Filtro.Movimientos.CodigoReserva).MaximumLength(50);
            });
        });
    }
}
