using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Cancha.DetallePago
{
    public class DeleteDetallePagoCommandValidator : CommandValidatorBase<DeleteDetallePagoCommand>
    {
        private readonly IRepository<Entity.Models.DetallePago> _repositoryBase;
        public DeleteDetallePagoCommandValidator(IRepository<Entity.Models.DetallePago> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredField(x => x.Id, Resources.Cancha.DetallePago.IdDetallePago)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(DeleteDetallePagoCommand command, int id, ValidationContext<DeleteDetallePagoCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdDetallePago == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.DeleteRecordNotFound);
            return true;
        }
    }
}
