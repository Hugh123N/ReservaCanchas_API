using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Pago
{
    public class GetPagoQueryValidator : QueryValidatorBase<GetPagoQuery>
    {
        private readonly IRepository<Entity.Models.Pago> _PagoRepository;

        public GetPagoQueryValidator(IRepository<Entity.Models.Pago> PagoRepository)
        {
            _PagoRepository = PagoRepository;

            RequiredField(x => x.Id, Resources.Cancha.Pago.IdPago)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetPagoQuery command, int id, ValidationContext<GetPagoQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _PagoRepository.FindAll().Where(x => x.IdPago == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
