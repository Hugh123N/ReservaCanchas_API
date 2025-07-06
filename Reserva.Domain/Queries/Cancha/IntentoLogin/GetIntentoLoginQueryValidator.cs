using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.IntentoLogin
{
    public class GetIntentoLoginQueryValidator : QueryValidatorBase<GetIntentoLoginQuery>
    {
        private readonly IRepository<Entity.Models.IntentoLogin> _IntentoLoginRepository;

        public GetIntentoLoginQueryValidator(IRepository<Entity.Models.IntentoLogin> IntentoLoginRepository)
        {
            _IntentoLoginRepository = IntentoLoginRepository;

            RequiredField(x => x.Id, "El campo 'IdIntentoLogin' es obligatorio.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetIntentoLoginQuery command, int id, ValidationContext<GetIntentoLoginQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _IntentoLoginRepository.FindAll().Where(x => x.IdIntentoLogin == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
