using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.Reserva
{
    public class CreateReservaCommandValidator : CommandValidatorBase<CreateReservaCommand>
    {

        private readonly IRepository<Entity.Cancha> _CanchaRepository;
        public CreateReservaCommandValidator(IRepository<Entity.Cancha> CanchaRepository)
        {
            _CanchaRepository = CanchaRepository;
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                RuleFor(x => x.CreateDto.IdUsuario)
                    .NotEmpty()
                    .WithMessage("El ID del usuario es obligatorio.");

                RuleFor(x => x.CreateDto.IdCancha)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();

                RuleFor(x => x.CreateDto.Fecha)
                    .NotEmpty()
                    .WithMessage("La fecha de la reserva es obligatoria.")
                    .Must(BeValidDate)
                    .WithMessage("La fecha de la reserva no puede ser una fecha pasada.");

                RuleFor(x => x.CreateDto.Monto)
                    .NotNull()
                    .WithMessage("El monto es obligatorio.")
                    .GreaterThan(0)
                    .WithMessage("El monto debe ser mayor a 0.");

                RuleFor(x => x.CreateDto.HoraInicio)
                    .NotEmpty()
                    .WithMessage("La hora de inicio es obligatoria.");

                RuleFor(x => x.CreateDto.HoraFin)
                    .NotEmpty()
                    .WithMessage("La hora de fin es obligatoria.");

                RuleFor(x => x.CreateDto)
                    .Must(dto => dto.HoraFin > dto.HoraInicio)
                    .WithMessage("La hora de fin debe ser mayor que la hora de inicio.");

                RuleFor(x => x.CreateDto)
                    .Must(dto => {
                        var duracion = dto.HoraFin - dto.HoraInicio;
                        return duracion.TotalMinutes >= 60 && duracion.TotalHours <= 24;
                    })
                    .WithMessage("La duración de la reserva debe ser de al menos 30 minutos y no más de 24 horas.");
            });
        }

        protected async Task<bool> ValidateExistenceAsync(CreateReservaCommand command, int id, ValidationContext<CreateReservaCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _CanchaRepository.FindAll().Where(x => x.IdCancha == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }

        private bool BeValidDate(DateTime fecha)
        {
            // Obtener la fecha de hoy sin hora
            var hoy = DateTime.Today;
            return fecha.Date >= hoy;
        }
    }
}
