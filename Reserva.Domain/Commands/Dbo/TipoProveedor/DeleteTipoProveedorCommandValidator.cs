using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.TipoProveedor
{
    public class DeleteTipoProveedorCommandValidator : CommandValidatorBase<DeleteTipoProveedorCommand>
    {
        private readonly IRepository<Entity.TipoProveedor> _repositoryBase;
        public DeleteTipoProveedorCommandValidator(IRepository<Entity.TipoProveedor> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredField(x => x.Id, Resources.Dbo.TipoProveedor.IdTipoProveedor)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(DeleteTipoProveedorCommand command, int id, ValidationContext<DeleteTipoProveedorCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdTipoProveedor == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.DeleteRecordNotFound);
            return true;
        }
    }
}
