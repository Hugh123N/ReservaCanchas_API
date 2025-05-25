using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Cancha.Usuario
{
    public class UpdateUsuarioCommandValidator : CommandValidatorBase<UpdateUsuarioCommand>
    {
        private readonly IRepository<Entity.Models.Usuario> _repositoryBase;
        public UpdateUsuarioCommandValidator(IRepository<Entity.Models.Usuario> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdUsuario, Resources.Cancha.Usuario.IdUsuario)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdUsuario)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Cancha.Usuario.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Cancha.Usuario.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateUsuarioCommand command, int id, ValidationContext<UpdateUsuarioCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdUsuario == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
