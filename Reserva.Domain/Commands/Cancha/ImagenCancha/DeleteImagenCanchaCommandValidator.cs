using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Cancha.ImagenCancha
{
    public class DeleteImagenCanchaCommandValidator : CommandValidatorBase<DeleteImagenCanchaCommand>
    {
        private readonly IRepository<Entity.Models.ImagenCancha> _repositoryBase;
        public DeleteImagenCanchaCommandValidator(IRepository<Entity.Models.ImagenCancha> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredField(x => x.Id, Resources.Cancha.ImagenCancha.IdImagenCancha)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(DeleteImagenCanchaCommand command, int id, ValidationContext<DeleteImagenCanchaCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdImagenCancha == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.DeleteRecordNotFound);
            return true;
        }
    }
}
