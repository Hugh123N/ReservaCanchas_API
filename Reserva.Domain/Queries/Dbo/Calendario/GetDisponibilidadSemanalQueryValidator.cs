using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.Calendario
{
    public class GetDisponibilidadSemanalQueryValidator : QueryValidatorBase<GetDisponibilidadSemanalQuery>
    {
        private readonly IRepository<Entity.Cancha> _canchaRepository;

        public GetDisponibilidadSemanalQueryValidator(
            IRepository<Entity.Cancha> canchaRepository)
        {
            _canchaRepository = canchaRepository;

            // IdCancha debe ser mayor a 0
            RuleFor(x => x.IdCancha)
                .GreaterThan(0)
                .WithMessage("El ID de la cancha debe ser mayor a 0");

            // IdCancha debe existir
            RuleFor(x => x.IdCancha)
                .MustAsync(ValidateCanchaExistsAsync)
                .WithMessage("La cancha no existe");

            // FechaInicio debe ser lunes
            RuleFor(x => x.FechaInicio)
                .Must(fecha => fecha.DayOfWeek == DayOfWeek.Monday)
                .WithMessage("La fecha de inicio debe ser un lunes");

            // FechaFin debe ser domingo
            RuleFor(x => x.FechaFin)
                .Must(fecha => fecha.DayOfWeek == DayOfWeek.Sunday)
                .WithMessage("La fecha de fin debe ser un domingo");

            // Rango debe ser exactamente 6 días (lunes a domingo)
            RuleFor(x => x)
                .Must(query => (query.FechaFin.Date - query.FechaInicio.Date).TotalDays == 6)
                .WithMessage("El rango debe ser exactamente 6 días (de lunes a domingo)");
        }

        private async Task<bool> ValidateCanchaExistsAsync(int idCancha, CancellationToken cancellationToken)
        {
            return await _canchaRepository
                .FindAll()
                .AnyAsync(c => c.IdCancha == idCancha && c.Activo, cancellationToken);
        }

    }
}
