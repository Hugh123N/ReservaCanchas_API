using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Cancha.EstadoReserva
{
    public class DeleteEstadoReservaCommandValidator : CommandValidatorBase<DeleteEstadoReservaCommand>
    {
        private readonly IRepository<Entity.Models.EstadoReserva> _repositoryBase;
        public DeleteEstadoReservaCommandValidator(IRepository<Entity.Models.EstadoReserva> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredField(x => x.Id, Resources.Cancha.EstadoReserva.IdEstadoReserva)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(DeleteEstadoReservaCommand command, int id, ValidationContext<DeleteEstadoReservaCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdEstadoReserva == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.DeleteRecordNotFound);
            return true;
        }
    }
}
