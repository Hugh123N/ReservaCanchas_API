using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Cancha.Rol
{
    public class UpdateRolCommandValidator : CommandValidatorBase<UpdateRolCommand>
    {
        private readonly IRepository<Entity.Models.Rol> _repositoryBase;
        public UpdateRolCommandValidator(IRepository<Entity.Models.Rol> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdRol, Resources.Cancha.Rol.IdRol)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdRol)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Cancha.Rol.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Cancha.Rol.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateRolCommand command, int id, ValidationContext<UpdateRolCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdRol == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
