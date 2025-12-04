using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.TipoDeporte
{
    public class DeleteTipoDeporteCommandValidator : CommandValidatorBase<DeleteTipoDeporteCommand>
    {
        private readonly IRepository<Entity.TipoDeporte> _repositoryBase;
        public DeleteTipoDeporteCommandValidator(IRepository<Entity.TipoDeporte> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredField(x => x.Id, Resources.Dbo.TipoDeporte.IdTipoDeporte)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(DeleteTipoDeporteCommand command, int id, ValidationContext<DeleteTipoDeporteCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdTipoDeporte == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.DeleteRecordNotFound);
            return true;
        }
    }
}
