using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Cancha.TipoProveedor
{
    public class UpdateTipoProveedorCommandValidator : CommandValidatorBase<UpdateTipoProveedorCommand>
    {
        private readonly IRepository<Entity.Models.TipoProveedor> _repositoryBase;
        public UpdateTipoProveedorCommandValidator(IRepository<Entity.Models.TipoProveedor> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdTipoProveedor, Resources.Cancha.TipoProveedor.IdTipoProveedor)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdTipoProveedor)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Cancha.TipoProveedor.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Cancha.TipoProveedor.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateTipoProveedorCommand command, int id, ValidationContext<UpdateTipoProveedorCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdTipoProveedor == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
