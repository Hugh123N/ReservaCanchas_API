using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.EstadoCancha
{
    public class GetEstadoCanchaQueryValidator : QueryValidatorBase<GetEstadoCanchaQuery>
    {
        private readonly IRepository<Entity.Models.EstadoCancha> _EstadoCanchaRepository;

        public GetEstadoCanchaQueryValidator(IRepository<Entity.Models.EstadoCancha> EstadoCanchaRepository)
        {
            _EstadoCanchaRepository = EstadoCanchaRepository;

            RequiredField(x => x.Id, Resources.Cancha.EstadoCancha.IdEstadoCancha)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetEstadoCanchaQuery command, int id, ValidationContext<GetEstadoCanchaQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _EstadoCanchaRepository.FindAll().Where(x => x.IdEstadoCancha == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
