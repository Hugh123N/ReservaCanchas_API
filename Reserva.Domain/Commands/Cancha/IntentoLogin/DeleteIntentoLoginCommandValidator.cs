using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Cancha.IntentoLogin
{
    public class DeleteIntentoLoginCommandValidator : CommandValidatorBase<DeleteIntentoLoginCommand>
    {
        private readonly IRepository<Entity.Models.IntentoLogin> _repositoryBase;
        public DeleteIntentoLoginCommandValidator(IRepository<Entity.Models.IntentoLogin> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredField(x => x.Id, "El campo 'Id' es obligatorio.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(ValidateExistenceAsync)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateExistenceAsync(DeleteIntentoLoginCommand command, int id, ValidationContext<DeleteIntentoLoginCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdIntentoLogin == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.DeleteRecordNotFound);
            return true;
        }
    }
}
