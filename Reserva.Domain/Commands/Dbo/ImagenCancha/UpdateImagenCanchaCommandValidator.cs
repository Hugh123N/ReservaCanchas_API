using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.ImagenCancha
{
    public class UpdateImagenCanchaCommandValidator : CommandValidatorBase<UpdateImagenCanchaCommand>
    {
        private readonly IRepository<Entity.ImagenCancha> _repositoryBase;
        public UpdateImagenCanchaCommandValidator(IRepository<Entity.ImagenCancha> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                // El IdImagenCancha ahora es nullable, solo validar existencia cuando tiene valor
                RuleFor(x => x.UpdateDto.IdImagenCancha)
                    .MustAsync(ValidateExistenceAsync)
                    .WithCustomValidationMessage()
                    .When(x => x.UpdateDto.IdImagenCancha.HasValue);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateImagenCanchaCommand command, int? id, ValidationContext<UpdateImagenCanchaCommand> context, CancellationToken cancellationToken)
        {
            if (!id.HasValue) return true;

            var exists = await _repositoryBase.FindAll().Where(x => x.IdImagenCancha == id.Value).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
