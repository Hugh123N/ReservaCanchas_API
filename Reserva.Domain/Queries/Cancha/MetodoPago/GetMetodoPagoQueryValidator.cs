using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.MetodoPago
{
    public class GetMetodoPagoQueryValidator : QueryValidatorBase<GetMetodoPagoQuery>
    {
        private readonly IRepository<Entity.Models.MetodoPago> _MetodoPagoRepository;

        public GetMetodoPagoQueryValidator(IRepository<Entity.Models.MetodoPago> MetodoPagoRepository)
        {
            _MetodoPagoRepository = MetodoPagoRepository;

            RequiredField(x => x.Id, Resources.Cancha.MetodoPago.IdMetodoPago)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetMetodoPagoQuery command, int id, ValidationContext<GetMetodoPagoQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _MetodoPagoRepository.FindAll().Where(x => x.IdMetodoPago == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
