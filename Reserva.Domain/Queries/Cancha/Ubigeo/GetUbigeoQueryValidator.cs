using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Ubigeo
{
    public class GetUbigeoQueryValidator : QueryValidatorBase<GetUbigeoQuery>
    {
        private readonly IRepository<Entity.Models.Ubigeo> _UbigeoRepository;

        public GetUbigeoQueryValidator(IRepository<Entity.Models.Ubigeo> UbigeoRepository)
        {
            _UbigeoRepository = UbigeoRepository;

            RequiredField(x => x.Id, Resources.Cancha.Ubigeo.IdUbigeo)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetUbigeoQuery command, string id, ValidationContext<GetUbigeoQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _UbigeoRepository.FindAll().Where(x => x.CodigoUbigeo == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
