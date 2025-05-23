using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Zona
{
    public class GetZonaQueryValidator : QueryValidatorBase<GetZonaQuery>
    {
        private readonly IRepository<Entity.Models.Zona> _ZonaRepository;

        public GetZonaQueryValidator(IRepository<Entity.Models.Zona> ZonaRepository)
        {
            _ZonaRepository = ZonaRepository;

            RequiredField(x => x.Id, Resources.Cancha.Zona.IdZona)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetZonaQuery command, int id, ValidationContext<GetZonaQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _ZonaRepository.FindAll().Where(x => x.IdZona == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
