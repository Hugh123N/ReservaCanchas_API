using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.HorarioCancha
{
    public class GetHorarioCanchaQueryValidator : QueryValidatorBase<GetHorarioCanchaQuery>
    {
        private readonly IRepository<Entity.HorarioCancha> _HorarioCanchaRepository;

        public GetHorarioCanchaQueryValidator(IRepository<Entity.HorarioCancha> HorarioCanchaRepository)
        {
            _HorarioCanchaRepository = HorarioCanchaRepository;

            RequiredField(x => x.Id, Resources.Dbo.HorarioCancha.IdHorarioCancha)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetHorarioCanchaQuery command, int id, ValidationContext<GetHorarioCanchaQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _HorarioCanchaRepository.FindAll().Where(x => x.IdHorarioCancha == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
