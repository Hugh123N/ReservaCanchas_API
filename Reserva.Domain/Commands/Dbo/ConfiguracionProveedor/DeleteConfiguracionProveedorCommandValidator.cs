using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.ConfiguracionProveedor
{
    public class DeleteConfiguracionProveedorCommandValidator : CommandValidatorBase<DeleteConfiguracionProveedorCommand>
    {
        private readonly IRepository<Entity.ConfiguracionProveedor> _repositoryBase;
        public DeleteConfiguracionProveedorCommandValidator(IRepository<Entity.ConfiguracionProveedor> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredField(x => x.Id, Resources.Dbo.ConfiguracionProveedor.IdConfiguracionProveedor)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(DeleteConfiguracionProveedorCommand command, int id, ValidationContext<DeleteConfiguracionProveedorCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdConfiguracionProveedor == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.DeleteRecordNotFound);
            return true;
        }
    }
}
