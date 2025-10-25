using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.GananciaProveedor
{
    public class UpdateGananciaProveedorCommandValidator : CommandValidatorBase<UpdateGananciaProveedorCommand>
    {
        private readonly IRepository<Entity.GananciaProveedor> _repositoryBase;
        public UpdateGananciaProveedorCommandValidator(IRepository<Entity.GananciaProveedor> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdGananciaProveedor, Resources.Dbo.GananciaProveedor.IdGananciaProveedor)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdGananciaProveedor)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Dbo.GananciaProveedor.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Dbo.GananciaProveedor.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateGananciaProveedorCommand command, int id, ValidationContext<UpdateGananciaProveedorCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdGananciaProveedor == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
