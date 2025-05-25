using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Cancha.GananciaProveedor
{
    public class UpdateGananciaProveedorCommandValidator : CommandValidatorBase<UpdateGananciaProveedorCommand>
    {
        private readonly IRepository<Entity.Models.GananciaProveedor> _repositoryBase;
        public UpdateGananciaProveedorCommandValidator(IRepository<Entity.Models.GananciaProveedor> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdGananciaProveedor, Resources.Cancha.GananciaProveedor.IdGananciaProveedor)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdGananciaProveedor)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Cancha.GananciaProveedor.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Cancha.GananciaProveedor.FechaIngreso);
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
