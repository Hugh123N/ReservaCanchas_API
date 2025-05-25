using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.EstadoPago
{
    public class GetEstadoPagoQueryValidator : QueryValidatorBase<GetEstadoPagoQuery>
    {
        private readonly IRepository<Entity.Models.EstadoPago> _EstadoPagoRepository;

        public GetEstadoPagoQueryValidator(IRepository<Entity.Models.EstadoPago> EstadoPagoRepository)
        {
            _EstadoPagoRepository = EstadoPagoRepository;

            RequiredField(x => x.Id, Resources.Cancha.EstadoPago.IdEstadoPago)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetEstadoPagoQuery command, int id, ValidationContext<GetEstadoPagoQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _EstadoPagoRepository.FindAll().Where(x => x.IdEstadoPago == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
