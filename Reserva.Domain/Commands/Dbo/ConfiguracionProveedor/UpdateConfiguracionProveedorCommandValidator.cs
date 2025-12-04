using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.ConfiguracionProveedor
{
    public class UpdateConfiguracionProveedorCommandValidator : CommandValidatorBase<UpdateConfiguracionProveedorCommand>
    {
        private readonly IRepository<Entity.ConfiguracionProveedor> _repositoryBase;
        public UpdateConfiguracionProveedorCommandValidator(IRepository<Entity.ConfiguracionProveedor> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdConfiguracionProveedor, Resources.Dbo.ConfiguracionProveedor.IdConfiguracionProveedor)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdConfiguracionProveedor)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Dbo.ConfiguracionProveedor.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Dbo.ConfiguracionProveedor.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateConfiguracionProveedorCommand command, int id, ValidationContext<UpdateConfiguracionProveedorCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdConfiguracionProveedor == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
