using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.Hora
{
    public class GetHoraQueryValidator : QueryValidatorBase<GetHoraQuery>
    {
        private readonly IRepository<Entity.Hora> _HoraRepository;

        public GetHoraQueryValidator(IRepository<Entity.Hora> HoraRepository)
        {
            _HoraRepository = HoraRepository;

            RequiredField(x => x.Id, Resources.Dbo.Hora.IdHora)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetHoraQuery command, int id, ValidationContext<GetHoraQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _HoraRepository.FindAll().Where(x => x.IdHora == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
