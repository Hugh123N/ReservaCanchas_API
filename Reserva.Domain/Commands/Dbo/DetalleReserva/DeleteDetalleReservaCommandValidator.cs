using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.DetalleReserva
{
    public class DeleteDetalleReservaCommandValidator : CommandValidatorBase<DeleteDetalleReservaCommand>
    {
        private readonly IRepository<Entity.DetalleReserva> _repositoryBase;
        public DeleteDetalleReservaCommandValidator(IRepository<Entity.DetalleReserva> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredField(x => x.Id, Resources.Dbo.DetalleReserva.IdDetalleReserva)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(DeleteDetalleReservaCommand command, int id, ValidationContext<DeleteDetalleReservaCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdDetalleReserva == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.DeleteRecordNotFound);
            return true;
        }
    }
}
