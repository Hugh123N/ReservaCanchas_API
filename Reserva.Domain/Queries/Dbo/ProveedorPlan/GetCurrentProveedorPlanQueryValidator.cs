using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.ProveedorPlan
{
    public class GetCurrentProveedorPlanQueryValidator : QueryValidatorBase<GetCurrentProveedorPlanQuery>
    {
        private readonly IRepository<Entity.Proveedor> _ProveedorRepository;
        public GetCurrentProveedorPlanQueryValidator(IRepository<Entity.Proveedor> ProveedorRepository)
        {
             _ProveedorRepository = ProveedorRepository;

            RequiredField(x => x.IdProveedor, Resources.Dbo.ProveedorPlan.IdProveedorPlan)
                .DependentRules(() =>
                {
                    RuleFor(x => x.IdProveedor)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetCurrentProveedorPlanQuery command, int id, ValidationContext<GetCurrentProveedorPlanQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _ProveedorRepository.FindAll().Where(x => x.IdProveedor == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}