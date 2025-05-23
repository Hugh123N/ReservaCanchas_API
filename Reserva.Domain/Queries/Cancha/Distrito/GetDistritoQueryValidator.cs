using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Distrito
{
    public class GetDistritoQueryValidator : QueryValidatorBase<GetDistritoQuery>
    {
        private readonly IRepository<Entity.Models.Distrito> _DistritoRepository;

        public GetDistritoQueryValidator(IRepository<Entity.Models.Distrito> DistritoRepository)
        {
            _DistritoRepository = DistritoRepository;

            RequiredField(x => x.Id, Resources.Cancha.Distrito.IdDistrito)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetDistritoQuery command, int id, ValidationContext<GetDistritoQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _DistritoRepository.FindAll().Where(x => x.IdDistrito == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
