using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.DetallePago
{
    public class GetDetallePagoQueryValidator : QueryValidatorBase<GetDetallePagoQuery>
    {
        private readonly IRepository<Entity.Models.DetallePago> _DetallePagoRepository;

        public GetDetallePagoQueryValidator(IRepository<Entity.Models.DetallePago> DetallePagoRepository)
        {
            _DetallePagoRepository = DetallePagoRepository;

            RequiredField(x => x.Id, Resources.Cancha.DetallePago.IdDetallePago)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetDetallePagoQuery command, int id, ValidationContext<GetDetallePagoQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _DetallePagoRepository.FindAll().Where(x => x.IdDetallePago == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
