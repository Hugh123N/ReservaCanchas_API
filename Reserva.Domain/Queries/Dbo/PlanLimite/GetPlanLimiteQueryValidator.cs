using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.PlanLimite
{
    public class GetPlanLimiteQueryValidator : QueryValidatorBase<GetPlanLimiteQuery>
    {
        private readonly IRepository<Entity.PlanLimite> _PlanLimiteRepository;

        public GetPlanLimiteQueryValidator(IRepository<Entity.PlanLimite> PlanLimiteRepository)
        {
            _PlanLimiteRepository = PlanLimiteRepository;

            RequiredField(x => x.Id, Resources.Dbo.PlanLimite.IdPlanLimite)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetPlanLimiteQuery command, int id, ValidationContext<GetPlanLimiteQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _PlanLimiteRepository.FindAll().Where(x => x.IdPlanLimite == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
