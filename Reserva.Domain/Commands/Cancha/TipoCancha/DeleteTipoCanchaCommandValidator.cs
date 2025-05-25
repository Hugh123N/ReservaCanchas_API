using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Cancha.TipoCancha
{
    public class DeleteTipoCanchaCommandValidator : CommandValidatorBase<DeleteTipoCanchaCommand>
    {
        private readonly IRepository<Entity.Models.TipoCancha> _repositoryBase;
        public DeleteTipoCanchaCommandValidator(IRepository<Entity.Models.TipoCancha> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredField(x => x.Id, Resources.Cancha.TipoCancha.IdTipoCancha)
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
