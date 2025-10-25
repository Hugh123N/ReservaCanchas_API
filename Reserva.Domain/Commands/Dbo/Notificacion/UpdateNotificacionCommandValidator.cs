using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.Notificacion
{
    public class UpdateNotificacionCommandValidator : CommandValidatorBase<UpdateNotificacionCommand>
    {
        private readonly IRepository<Entity.Notificacion> _repositoryBase;
        public UpdateNotificacionCommandValidator(IRepository<Entity.Notificacion> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdNotificacion, Resources.Dbo.Notificacion.IdNotificacion)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdNotificacion)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Dbo.Notificacion.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Dbo.Notificacion.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateNotificacionCommand command, int id, ValidationContext<UpdateNotificacionCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdNotificacion == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
