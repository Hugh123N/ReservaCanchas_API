using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Cancha.Usuario
{
    public class DeleteUsuarioCommandValidator : CommandValidatorBase<DeleteUsuarioCommand>
    {
        private readonly IRepository<Entity.Models.Usuario> _repositoryBase;
        public DeleteUsuarioCommandValidator(IRepository<Entity.Models.Usuario> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredField(x => x.Id, Resources.Cancha.Usuario.IdUsuario)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(DeleteUsuarioCommand command, int id, ValidationContext<DeleteUsuarioCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdUsuario == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.DeleteRecordNotFound);
            return true;
        }
    }
}
