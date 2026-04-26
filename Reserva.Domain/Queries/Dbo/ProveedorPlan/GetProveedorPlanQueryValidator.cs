using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.ProveedorPlan
{
    public class GetProveedorPlanQueryValidator : QueryValidatorBase<GetProveedorPlanQuery>
    {
        private readonly IRepository<Entity.ProveedorPlan> _ProveedorPlanRepository;

        public GetProveedorPlanQueryValidator(IRepository<Entity.ProveedorPlan> ProveedorPlanRepository)
        {
            _ProveedorPlanRepository = ProveedorPlanRepository;

            RequiredField(x => x.Id, Resources.Dbo.ProveedorPlan.IdProveedorPlan)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetProveedorPlanQuery command, int id, ValidationContext<GetProveedorPlanQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _ProveedorPlanRepository.FindAll().Where(x => x.IdProveedorPlan == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
