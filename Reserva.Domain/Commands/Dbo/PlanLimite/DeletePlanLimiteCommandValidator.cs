using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.PlanLimite
{
    public class DeletePlanLimiteCommandValidator : CommandValidatorBase<DeletePlanLimiteCommand>
    {
        private readonly IRepository<Entity.PlanLimite> _repositoryBase;
        public DeletePlanLimiteCommandValidator(IRepository<Entity.PlanLimite> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredField(x => x.Id, Resources.Dbo.PlanLimite.IdPlanLimite)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(DeletePlanLimiteCommand command, int id, ValidationContext<DeletePlanLimiteCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdPlanLimite == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.DeleteRecordNotFound);
            return true;
        }
    }
}
