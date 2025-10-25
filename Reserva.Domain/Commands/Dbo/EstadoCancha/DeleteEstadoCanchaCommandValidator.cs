using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.EstadoCancha
{
    public class DeleteEstadoCanchaCommandValidator : CommandValidatorBase<DeleteEstadoCanchaCommand>
    {
        private readonly IRepository<Entity.EstadoCancha> _repositoryBase;
        public DeleteEstadoCanchaCommandValidator(IRepository<Entity.EstadoCancha> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredField(x => x.Id, Resources.Dbo.EstadoCancha.IdEstadoCancha)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(DeleteEstadoCanchaCommand command, int id, ValidationContext<DeleteEstadoCanchaCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdEstadoCancha == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.DeleteRecordNotFound);
            return true;
        }
    }
}
