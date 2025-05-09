using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Entity.Models;

namespace Reserva.Domain.Commands.Dbo.DiaSemana
{
    public class DeleteDiaSemanaCommandValidator : CommandValidatorBase<DeleteDiaSemanaCommand>
    {
        private readonly ReservaCanchasContext _context;
        public DeleteDiaSemanaCommandValidator(ReservaCanchasContext context)
        {
            _context = context;

            RequiredField(x => x.Id, "Id del día de semana")
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(DeleteDiaSemanaCommand command, int id, ValidationContext<DeleteDiaSemanaCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _context.DiaSemanas.Where(x => x.IdDia == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, AppMessages.NotFound);
            return true;
        }
    }
}
