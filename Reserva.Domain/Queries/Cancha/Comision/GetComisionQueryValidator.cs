using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Comision
{
    public class GetComisionQueryValidator : QueryValidatorBase<GetComisionQuery>
    {
        private readonly IRepository<Entity.Models.Comision> _ComisionRepository;

        public GetComisionQueryValidator(IRepository<Entity.Models.Comision> ComisionRepository)
        {
            _ComisionRepository = ComisionRepository;

            RequiredField(x => x.Id, Resources.Cancha.Comision.IdComision)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetComisionQuery command, int id, ValidationContext<GetComisionQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _ComisionRepository.FindAll().Where(x => x.IdComision == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
