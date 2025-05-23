using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Cancha.EstadoUsuario
{
    public class DeleteEstadoUsuarioCommandValidator : CommandValidatorBase<DeleteEstadoUsuarioCommand>
    {
        private readonly IRepository<Entity.Models.EstadoUsuario> _repositoryBase;
        public DeleteEstadoUsuarioCommandValidator(IRepository<Entity.Models.EstadoUsuario> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredField(x => x.Id, Resources.Cancha.EstadoUsuario.IdEstadoUsuario)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(DeleteEstadoUsuarioCommand command, int id, ValidationContext<DeleteEstadoUsuarioCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdEstadoUsuario == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.DeleteRecordNotFound);
            return true;
        }
    }
}
