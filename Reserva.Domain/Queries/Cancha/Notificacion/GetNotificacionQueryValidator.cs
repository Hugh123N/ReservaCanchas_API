using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Notificacion
{
    public class GetNotificacionQueryValidator : QueryValidatorBase<GetNotificacionQuery>
    {
        private readonly IRepository<Entity.Models.Notificacion> _NotificacionRepository;

        public GetNotificacionQueryValidator(IRepository<Entity.Models.Notificacion> NotificacionRepository)
        {
            _NotificacionRepository = NotificacionRepository;

            RequiredField(x => x.Id, Resources.Cancha.Notificacion.IdNotificacion)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(GetNotificacionQuery command, int id, ValidationContext<GetNotificacionQuery> context, CancellationToken cancellationToken)
        {
            var exists = await _NotificacionRepository.FindAll().Where(x => x.IdNotificacion == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }
    }
}
