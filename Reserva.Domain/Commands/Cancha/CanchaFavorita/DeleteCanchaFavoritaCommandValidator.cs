using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Cancha.CanchaFavorita
{
    public class DeleteCanchaFavoritaCommandValidator : CommandValidatorBase<DeleteCanchaFavoritaCommand>
    {
        private readonly IRepository<Entity.Models.CanchaFavorita> _repositoryBase;
        public DeleteCanchaFavoritaCommandValidator(IRepository<Entity.Models.CanchaFavorita> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredField(x => x.Id, Resources.Cancha.CanchaFavorita.IdCanchaFavorita)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(DeleteCanchaFavoritaCommand command, int id, ValidationContext<DeleteCanchaFavoritaCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdCancha == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.DeleteRecordNotFound);
            return true;
        }
    }
}
