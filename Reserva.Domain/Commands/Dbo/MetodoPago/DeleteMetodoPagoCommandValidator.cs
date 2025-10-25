using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.MetodoPago
{
    public class DeleteMetodoPagoCommandValidator : CommandValidatorBase<DeleteMetodoPagoCommand>
    {
        private readonly IRepository<Entity.MetodoPago> _repositoryBase;
        public DeleteMetodoPagoCommandValidator(IRepository<Entity.MetodoPago> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredField(x => x.Id, Resources.Dbo.MetodoPago.IdMetodoPago)
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
