using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Cancha.MetodoPago
{
    public class DeleteMetodoPagoCommandValidator : CommandValidatorBase<DeleteMetodoPagoCommand>
    {
        private readonly IRepository<Entity.Models.MetodoPago> _repositoryBase;
        public DeleteMetodoPagoCommandValidator(IRepository<Entity.Models.MetodoPago> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredField(x => x.Id, Resources.Cancha.MetodoPago.IdMetodoPago)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(DeleteMetodoPagoCommand command, int id, ValidationContext<DeleteMetodoPagoCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdMetodoPago == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.DeleteRecordNotFound);
            return true;
        }
    }
}
