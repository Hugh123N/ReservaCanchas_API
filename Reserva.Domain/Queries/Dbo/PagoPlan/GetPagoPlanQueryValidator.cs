using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.PagoPlan
{
    public class GetPagoPlanQueryValidator : QueryValidatorBase<GetPagoPlanQuery>
    {
        private readonly IRepository<Entity.PagoPlan> _PagoPlanRepository;

        public GetPagoPlanQueryValidator(IRepository<Entity.PagoPlan> PagoPlanRepository)
        {
            _PagoPlanRepository = PagoPlanRepository;

            RequiredField(x => x.Id, Resources.Dbo.PagoPlan.IdPagoPlan)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetPagoPlanQuery command, int id, ValidationContext<GetPagoPlanQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _PagoPlanRepository.FindAll().Where(x => x.IdPagoPlan == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
