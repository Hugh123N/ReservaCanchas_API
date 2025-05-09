using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Domain.Queries.Dbo.DiaSemana;
using Reserva.Entity.Models;

namespace Reserva.Domain.Queries.Dbo.DiaSemana
{
    public class GetDiaSemanaQueryValidator : QueryValidatorBase<GetDiaSemanaQuery>
    {
        private readonly ReservaCanchasContext _context;

        public GetDiaSemanaQueryValidator(ReservaCanchasContext context)
        {
            _context = context;

            RequiredField(x => x.Id, "El campo 'Id' del día de la semana es obligatorio.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithMessage("No se encontró un registro con el Id especificado.");
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetDiaSemanaQuery command, int id, ValidationContext<GetDiaSemanaQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _context.DiaSemanas.Where(x => x.IdDia == id)
                .AnyAsync(cancellationToken);

            return exists;
        }
    }
}
