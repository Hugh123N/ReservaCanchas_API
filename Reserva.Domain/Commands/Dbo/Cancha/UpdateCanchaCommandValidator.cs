using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.Cancha
{
    public class UpdateCanchaCommandValidator : CommandValidatorBase<UpdateCanchaCommand>
    {
        private readonly IRepository<Entity.Cancha> _repositoryBase;
        public UpdateCanchaCommandValidator(IRepository<Entity.Cancha> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdCancha, Resources.Dbo.Cancha.IdCancha)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdCancha)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });

                RequiredString(x => x.UpdateDto.Nombre, "El nombre de la cancha", 3, 100);
                RequiredString(x => x.UpdateDto.Direccion, "La Direccion de la cancha", 5, 200);

                RequiredField(x => x.UpdateDto.Precio, "El precio de la cancha")
                    .GreaterThan(0).WithMessage("El precio debe ser mayor a 0");

                RuleFor(x => x.UpdateDto.HorarioCanchas)
                    .NotNull().WithMessage("Debe proporcionar al menos un Horario.")
                    .Must(d => d.Any()).WithMessage("Debe proporcionar al menos una Hora.");

            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateCanchaCommand command, int id, ValidationContext<UpdateCanchaCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdCancha == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
