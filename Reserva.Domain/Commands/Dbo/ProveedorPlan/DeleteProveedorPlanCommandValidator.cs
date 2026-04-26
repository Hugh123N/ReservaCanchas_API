using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.ProveedorPlan
{
    public class DeleteProveedorPlanCommandValidator : CommandValidatorBase<DeleteProveedorPlanCommand>
    {
        private readonly IRepository<Entity.ProveedorPlan> _repositoryBase;
        public DeleteProveedorPlanCommandValidator(IRepository<Entity.ProveedorPlan> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredField(x => x.Id, Resources.Dbo.ProveedorPlan.IdProveedorPlan)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(DeleteProveedorPlanCommand command, int id, ValidationContext<DeleteProveedorPlanCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdProveedorPlan == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.DeleteRecordNotFound);
            return true;
        }
    }
}
