using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Cancha.DiaSemana
{
    public class DeleteDiaSemanaCommandValidator : CommandValidatorBase<DeleteDiaSemanaCommand>
    {
        private readonly IRepository<Entity.Models.DiaSemana> _repositoryBase;
        public DeleteDiaSemanaCommandValidator(IRepository<Entity.Models.DiaSemana> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredField(x => x.Id, Resources.Cancha.DiaSemana.IdDiaSemana)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(DeleteDiaSemanaCommand command, int id, ValidationContext<DeleteDiaSemanaCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdDiaSemana == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.DeleteRecordNotFound);
            return true;
        }
    }
}
