using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Cancha.Notificacion
{
    public class DeleteNotificacionCommandValidator : CommandValidatorBase<DeleteNotificacionCommand>
    {
        private readonly IRepository<Entity.Models.Notificacion> _repositoryBase;
        public DeleteNotificacionCommandValidator(IRepository<Entity.Models.Notificacion> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredField(x => x.Id, Resources.Cancha.Notificacion.IdNotificacion)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(DeleteNotificacionCommand command, int id, ValidationContext<DeleteNotificacionCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdNotificacion == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.DeleteRecordNotFound);
            return true;
        }
    }
}
