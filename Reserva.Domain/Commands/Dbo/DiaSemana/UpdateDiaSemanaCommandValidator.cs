using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Entity.Models;

namespace Reserva.Domain.Commands.Dbo.DiaSemana
{
    public class UpdateDiaSemanaCommandValidator : CommandValidatorBase<UpdateDiaSemanaCommand>
    {
        private readonly ReservaCanchasContext _context;
        public UpdateDiaSemanaCommandValidator(ReservaCanchasContext context)
        {
            _context = context;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdDia, "Id del día de semana")
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdDia)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Dbo.DiaSemana.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Dbo.DiaSemana.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateDiaSemanaCommand command, int id, ValidationContext<UpdateDiaSemanaCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _context.DiaSemanas.Where(x => x.IdDia == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, AppMessages.NotFound);
            return true;
        }
    }
}
