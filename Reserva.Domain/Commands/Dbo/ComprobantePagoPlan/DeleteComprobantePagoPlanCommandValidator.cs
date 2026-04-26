using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.ComprobantePagoPlan
{
    public class DeleteComprobantePagoPlanCommandValidator : CommandValidatorBase<DeleteComprobantePagoPlanCommand>
    {
        private readonly IRepository<Entity.ComprobantePagoPlan> _repositoryBase;
        public DeleteComprobantePagoPlanCommandValidator(IRepository<Entity.ComprobantePagoPlan> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredField(x => x.Id, Resources.Dbo.ComprobantePagoPlan.IdComprobantePagoPlan)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(DeleteComprobantePagoPlanCommand command, int id, ValidationContext<DeleteComprobantePagoPlanCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdComprobantePagoPlan == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.DeleteRecordNotFound);
            return true;
        }
    }
}
