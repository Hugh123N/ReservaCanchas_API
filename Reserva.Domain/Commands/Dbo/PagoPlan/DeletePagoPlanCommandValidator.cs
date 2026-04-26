using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.PagoPlan
{
    public class DeletePagoPlanCommandValidator : CommandValidatorBase<DeletePagoPlanCommand>
    {
        private readonly IRepository<Entity.PagoPlan> _repositoryBase;
        public DeletePagoPlanCommandValidator(IRepository<Entity.PagoPlan> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredField(x => x.Id, Resources.Dbo.PagoPlan.IdPagoPlan)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(DeletePagoPlanCommand command, int id, ValidationContext<DeletePagoPlanCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdPagoPlan == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.DeleteRecordNotFound);
            return true;
        }
    }
}
