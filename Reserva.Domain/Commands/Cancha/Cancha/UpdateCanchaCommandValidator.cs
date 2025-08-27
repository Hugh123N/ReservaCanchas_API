using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Cancha.Cancha
{
    public class UpdateCanchaCommandValidator : CommandValidatorBase<UpdateCanchaCommand>
    {
        private readonly IRepository<Entity.Cancha> _repositoryBase;
        public UpdateCanchaCommandValidator(IRepository<Entity.Cancha> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdCancha, Resources.Cancha.Cancha.IdCancha)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdCancha)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });

                RequiredString(x => x.UpdateDto.Nombre, "El nombre de la cancha", 3, 100);
                RequiredString(x => x.UpdateDto.Ubicacion, "La ubicación de la cancha", 5, 200);

                RequiredField(x => x.UpdateDto.PrecioHora, "El precio de la cancha")
                    .GreaterThan(0).WithMessage("El precio debe ser mayor a 0");

                RuleFor(x => x.UpdateDto.Disponibilidades)
                    .NotNull().WithMessage("Debe proporcionar al menos una disponibilidad.")
                    .Must(d => d.Any()).WithMessage("Debe proporcionar al menos una disponibilidad.");

                RuleForEach(x => x.UpdateDto.Disponibilidades).ChildRules(dis =>
                {
                    dis.RuleFor(d => d.HoraInicio)
                       .LessThan(d => d.HoraFin)
                       .WithMessage("La hora de inicio debe ser menor a la hora de fin.");
                });
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
