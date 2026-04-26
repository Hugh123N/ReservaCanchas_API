using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.ProveedorPlan
{
    public class UpdateProveedorPlanCommandValidator : CommandValidatorBase<UpdateProveedorPlanCommand>
    {
        private readonly IRepository<Entity.ProveedorPlan> _repositoryBase;
        public UpdateProveedorPlanCommandValidator(IRepository<Entity.ProveedorPlan> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdProveedorPlan, Resources.Dbo.ProveedorPlan.IdProveedorPlan)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdProveedorPlan)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Dbo.ProveedorPlan.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Dbo.ProveedorPlan.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateProveedorPlanCommand command, int id, ValidationContext<UpdateProveedorPlanCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdProveedorPlan == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
