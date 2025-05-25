using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Cancha.Disponibilidad
{
    public class DeleteDisponibilidadCommandValidator : CommandValidatorBase<DeleteDisponibilidadCommand>
    {
        private readonly IRepository<Entity.Models.Disponibilidad> _repositoryBase;
        public DeleteDisponibilidadCommandValidator(IRepository<Entity.Models.Disponibilidad> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredField(x => x.Id, Resources.Cancha.Disponibilidad.IdDisponibilidad)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(DeleteDisponibilidadCommand command, int id, ValidationContext<DeleteDisponibilidadCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdDisponibilidad == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.DeleteRecordNotFound);
            return true;
        }
    }
}
