using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.EstadoProveedor
{
    public class DeleteEstadoProveedorCommandValidator : CommandValidatorBase<DeleteEstadoProveedorCommand>
    {
        private readonly IRepository<Entity.EstadoProveedor> _repositoryBase;
        public DeleteEstadoProveedorCommandValidator(IRepository<Entity.EstadoProveedor> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredField(x => x.Id, Resources.Dbo.EstadoProveedor.IdEstadoProveedor)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(DeleteEstadoProveedorCommand command, int id, ValidationContext<DeleteEstadoProveedorCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdEstadoProveedor == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.DeleteRecordNotFound);
            return true;
        }
    }
}
