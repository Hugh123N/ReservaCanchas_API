using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Provincia
{
    public class GetProvinciaQueryValidator : QueryValidatorBase<GetProvinciaQuery>
    {
        private readonly IRepository<Entity.Models.Provincia> _ProvinciaRepository;

        public GetProvinciaQueryValidator(IRepository<Entity.Models.Provincia> ProvinciaRepository)
        {
            _ProvinciaRepository = ProvinciaRepository;

            RequiredField(x => x.Id, Resources.Cancha.Provincia.IdProvincia)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetProvinciaQuery command, int id, ValidationContext<GetProvinciaQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _ProvinciaRepository.FindAll().Where(x => x.IdProvincia == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
