using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.DetalleReserva
{
    public class GetDetalleReservaQueryValidator : QueryValidatorBase<GetDetalleReservaQuery>
    {
        private readonly IRepository<Entity.DetalleReserva> _DetalleReservaRepository;

        public GetDetalleReservaQueryValidator(IRepository<Entity.DetalleReserva> DetalleReservaRepository)
        {
            _DetalleReservaRepository = DetalleReservaRepository;

            RequiredField(x => x.Id, Resources.Dbo.DetalleReserva.IdDetalleReserva)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetDetalleReservaQuery command, int id, ValidationContext<GetDetalleReservaQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _DetalleReservaRepository.FindAll().Where(x => x.IdDetalleReserva == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
