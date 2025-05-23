using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.DiaSemana
{
    public class GetDiaSemanaQueryValidator : QueryValidatorBase<GetDiaSemanaQuery>
    {
        private readonly IRepository<Entity.Models.DiaSemana> _DiaSemanaRepository;

        public GetDiaSemanaQueryValidator(IRepository<Entity.Models.DiaSemana> DiaSemanaRepository)
        {
            _DiaSemanaRepository = DiaSemanaRepository;

            RequiredField(x => x.Id, Resources.Cancha.DiaSemana.IdDiaSemana)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetDiaSemanaQuery command, int id, ValidationContext<GetDiaSemanaQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _DiaSemanaRepository.FindAll().Where(x => x.IdDiaSemana == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
