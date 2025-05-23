using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Reserva
{
    public class GetReservaQueryValidator : QueryValidatorBase<GetReservaQuery>
    {
        private readonly IRepository<Entity.Models.Reserva> _ReservaRepository;

        public GetReservaQueryValidator(IRepository<Entity.Models.Reserva> ReservaRepository)
        {
            _ReservaRepository = ReservaRepository;

            RequiredField(x => x.Id, Resources.Cancha.Reserva.IdReserva)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetReservaQuery command, int id, ValidationContext<GetReservaQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _ReservaRepository.FindAll().Where(x => x.IdReserva == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
