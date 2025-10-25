using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.EstadoReserva
{
    public class GetEstadoReservaQueryValidator : QueryValidatorBase<GetEstadoReservaQuery>
    {
        private readonly IRepository<Entity.EstadoReserva> _EstadoReservaRepository;

        public GetEstadoReservaQueryValidator(IRepository<Entity.EstadoReserva> EstadoReservaRepository)
        {
            _EstadoReservaRepository = EstadoReservaRepository;

            RequiredField(x => x.Id, Resources.Dbo.EstadoReserva.IdEstadoReserva)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetEstadoReservaQuery command, int id, ValidationContext<GetEstadoReservaQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _EstadoReservaRepository.FindAll().Where(x => x.IdEstadoReserva == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
