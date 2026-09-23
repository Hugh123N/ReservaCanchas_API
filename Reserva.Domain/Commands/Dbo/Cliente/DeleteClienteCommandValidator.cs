using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.Cliente
{
    public class DeleteClienteCommandValidator : CommandValidatorBase<DeleteClienteCommand>
    {
        private readonly IRepository<Entity.Cliente> _repositoryBase;
        public DeleteClienteCommandValidator(IRepository<Entity.Cliente> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            //RequiredField(x => x.Id)
            //    .DependentRules(() =>
            //    {
            //        RuleFor(x => x.Id)
            //            .MustAsync(ValidateExistenceAsync)
            //            .WithCustomValidationMessage();
            //    });
        }

        protected async Task<bool> ValidateExistenceAsync(DeleteClienteCommand command, int id, ValidationContext<DeleteClienteCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdCliente == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.DeleteRecordNotFound);
            return true;
        }
    }
}
