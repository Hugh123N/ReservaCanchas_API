using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.Usuario
{
    public class DeleteUsuarioCommandValidator : CommandValidatorBase<DeleteUsuarioCommand>
    {
        private readonly IRepository<Entity.AspNetUsers> _repositoryBase;
        public DeleteUsuarioCommandValidator(IRepository<Entity.AspNetUsers> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredField(x => x.Id, Resources.Dbo.Usuario.IdUsuario)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(DeleteUsuarioCommand command, Guid id, ValidationContext<DeleteUsuarioCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.Id == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.DeleteRecordNotFound);
            return true;
        }
    }
}
