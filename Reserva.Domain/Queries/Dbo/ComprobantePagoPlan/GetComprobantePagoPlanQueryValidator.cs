using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.ComprobantePagoPlan
{
    public class GetComprobantePagoPlanQueryValidator : QueryValidatorBase<GetComprobantePagoPlanQuery>
    {
        private readonly IRepository<Entity.ComprobantePagoPlan> _ComprobantePagoPlanRepository;

        public GetComprobantePagoPlanQueryValidator(IRepository<Entity.ComprobantePagoPlan> ComprobantePagoPlanRepository)
        {
            _ComprobantePagoPlanRepository = ComprobantePagoPlanRepository;

            RequiredField(x => x.Id, Resources.Dbo.ComprobantePagoPlan.IdComprobantePagoPlan)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetComprobantePagoPlanQuery command, int id, ValidationContext<GetComprobantePagoPlanQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _ComprobantePagoPlanRepository.FindAll().Where(x => x.IdComprobantePagoPlan == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
