using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.TipoCancha
{
    public class DeleteTipoCanchaCommandValidator : CommandValidatorBase<DeleteTipoCanchaCommand>
    {
        private readonly IRepository<Entity.TipoCancha> _repositoryBase;
        public DeleteTipoCanchaCommandValidator(IRepository<Entity.TipoCancha> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredField(x => x.Id, Resources.Dbo.TipoCancha.IdTipoCancha)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(DeleteTipoCanchaCommand command, int id, ValidationContext<DeleteTipoCanchaCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdTipoCancha == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.DeleteRecordNotFound);
            return true;
        }
    }
}
