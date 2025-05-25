using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Cancha.GananciaProveedor
{
    public class DeleteGananciaProveedorCommandValidator : CommandValidatorBase<DeleteGananciaProveedorCommand>
    {
        private readonly IRepository<Entity.Models.GananciaProveedor> _repositoryBase;
        public DeleteGananciaProveedorCommandValidator(IRepository<Entity.Models.GananciaProveedor> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredField(x => x.Id, Resources.Cancha.GananciaProveedor.IdGananciaProveedor)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(DeleteGananciaProveedorCommand command, int id, ValidationContext<DeleteGananciaProveedorCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdGananciaProveedor == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.DeleteRecordNotFound);
            return true;
        }
    }
}
