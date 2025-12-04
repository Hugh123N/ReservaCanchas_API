using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.TipoSuperficie
{
    public class DeleteTipoSuperficieCommandValidator : CommandValidatorBase<DeleteTipoSuperficieCommand>
    {
        private readonly IRepository<Entity.TipoSuperficie> _repositoryBase;
        public DeleteTipoSuperficieCommandValidator(IRepository<Entity.TipoSuperficie> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredField(x => x.Id, Resources.Dbo.TipoSuperficie.IdTipoSuperficie)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(DeleteTipoSuperficieCommand command, int id, ValidationContext<DeleteTipoSuperficieCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdTipoSuperficie == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.DeleteRecordNotFound);
            return true;
        }
    }
}
