using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Cancha
{
    public class GetCanchaQueryValidator : QueryValidatorBase<GetCanchaQuery>
    {
        private readonly IRepository<Entity.Models.Cancha> _CanchaRepository;

        public GetCanchaQueryValidator(IRepository<Entity.Models.Cancha> CanchaRepository)
        {
            _CanchaRepository = CanchaRepository;

            RequiredField(x => x.Id, Resources.Cancha.Cancha.IdCancha)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetCanchaQuery command, int id, ValidationContext<GetCanchaQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _CanchaRepository.FindAll().Where(x => x.IdCancha == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
