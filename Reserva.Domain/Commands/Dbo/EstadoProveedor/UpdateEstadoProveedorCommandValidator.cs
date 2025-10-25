using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.EstadoProveedor
{
    public class UpdateEstadoProveedorCommandValidator : CommandValidatorBase<UpdateEstadoProveedorCommand>
    {
        private readonly IRepository<Entity.EstadoProveedor> _repositoryBase;
        public UpdateEstadoProveedorCommandValidator(IRepository<Entity.EstadoProveedor> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdEstadoProveedor, Resources.Dbo.EstadoProveedor.IdEstadoProveedor)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdEstadoProveedor)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Dbo.EstadoProveedor.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Dbo.EstadoProveedor.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateEstadoProveedorCommand command, int id, ValidationContext<UpdateEstadoProveedorCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdEstadoProveedor == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
