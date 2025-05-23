using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Cancha.EstadoUsuario
{
    public class UpdateEstadoUsuarioCommandValidator : CommandValidatorBase<UpdateEstadoUsuarioCommand>
    {
        private readonly IRepository<Entity.Models.EstadoUsuario> _repositoryBase;
        public UpdateEstadoUsuarioCommandValidator(IRepository<Entity.Models.EstadoUsuario> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdEstadoUsuario, Resources.Cancha.EstadoUsuario.IdEstadoUsuario)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdEstadoUsuario)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Cancha.EstadoUsuario.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Cancha.EstadoUsuario.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateEstadoUsuarioCommand command, int id, ValidationContext<UpdateEstadoUsuarioCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdEstadoUsuario == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
