using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Disponibilidad
{
    public class GetDisponibilidadQueryValidator : QueryValidatorBase<GetDisponibilidadQuery>
    {
        private readonly IRepository<Entity.Models.Disponibilidad> _DisponibilidadRepository;

        public GetDisponibilidadQueryValidator(IRepository<Entity.Models.Disponibilidad> DisponibilidadRepository)
        {
            _DisponibilidadRepository = DisponibilidadRepository;

            RequiredField(x => x.Id, Resources.Cancha.Disponibilidad.IdDisponibilidad)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetDisponibilidadQuery command, int id, ValidationContext<GetDisponibilidadQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _DisponibilidadRepository.FindAll().Where(x => x.IdDisponibilidad == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
