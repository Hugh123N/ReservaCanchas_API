using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Cancha.ImagenCancha
{
    public class UpdateImagenCanchaCommandValidator : CommandValidatorBase<UpdateImagenCanchaCommand>
    {
        private readonly IRepository<Entity.Models.ImagenCancha> _repositoryBase;
        public UpdateImagenCanchaCommandValidator(IRepository<Entity.Models.ImagenCancha> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdImagenCancha, Resources.Cancha.ImagenCancha.IdImagenCancha)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdImagenCancha)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Cancha.ImagenCancha.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Cancha.ImagenCancha.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateImagenCanchaCommand command, int id, ValidationContext<UpdateImagenCanchaCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdImagenCancha == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
