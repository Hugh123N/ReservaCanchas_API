using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.PlanLimite
{
    public class UpdatePlanLimiteCommandValidator : CommandValidatorBase<UpdatePlanLimiteCommand>
    {
        private readonly IRepository<Entity.PlanLimite> _repositoryBase;
        public UpdatePlanLimiteCommandValidator(IRepository<Entity.PlanLimite> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdPlanLimite, Resources.Dbo.PlanLimite.IdPlanLimite)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdPlanLimite)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Dbo.PlanLimite.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Dbo.PlanLimite.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdatePlanLimiteCommand command, int id, ValidationContext<UpdatePlanLimiteCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdPlanLimite == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
