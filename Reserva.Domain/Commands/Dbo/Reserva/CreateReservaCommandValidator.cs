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
        private readonly IRepository<Entity.MetodoPago> _MetodoPagoRepository;
        public CreateReservaCommandValidator(IRepository<Entity.Cancha> CanchaRepository, IRepository<Entity.MetodoPago> metodoPagoRepository)
        {
            _CanchaRepository = CanchaRepository;
            _MetodoPagoRepository = metodoPagoRepository;
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                RuleFor(x => x.CreateDto.IdCliente)
                    .NotEmpty()
                    .WithMessage("El ID del Cliente es obligatorio.");

                RuleFor(x => x.CreateDto.IdCancha)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();

                RuleFor(x => x.CreateDto.CodigoMetodoPago)
                            .MustAsync(ValidateExistenceMetodoPagoAsync)
                            .WithCustomValidationMessage();

                RuleFor(x => x.CreateDto.FechaReserva)
                    .NotEmpty()
                    .WithMessage("La fecha de la reserva es obligatoria.")
                    .Must(BeValidDate)
                    .WithMessage("La fecha de la reserva no puede ser una fecha pasada.");

                RuleFor(x => x.CreateDto.MontoTotal)
                    .NotNull()
                    .WithMessage("El monto es obligatorio.")
                    .GreaterThan(0)
                    .WithMessage("El monto debe ser mayor a 0.");

                // Validar MontoAdelanto (solo si viene informado)
                When(x => x.CreateDto.MontoAdelanto.HasValue && x.CreateDto.MontoAdelanto.Value > 0, () =>
                {
                    RuleFor(x => x.CreateDto.MontoAdelanto)
                        .GreaterThan(0)
                        .WithMessage("El monto del adelanto debe ser mayor a 0.");

                    RuleFor(x => x.CreateDto)
                        .Must(dto => dto.MontoAdelanto <= dto.MontoTotal)
                        .WithMessage("El monto del adelanto no puede ser mayor que el monto total.");
                });
            });


            _MetodoPagoRepository = metodoPagoRepository;
        }

        protected async Task<bool> ValidateExistenceAsync(CreateReservaCommand command, int id, ValidationContext<CreateReservaCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _CanchaRepository.FindAll().Where(x => x.IdCancha == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }

        protected async Task<bool> ValidateExistenceMetodoPagoAsync(CreateReservaCommand command, string codigoMetodoPago, ValidationContext<CreateReservaCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _MetodoPagoRepository.FindAll().Where(x => x.Codigo == codigoMetodoPago).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.GetRecordNotFound);
            return true;
        }

        private bool BeValidDate(DateTimeOffset fecha)
        {
            // Obtener la fecha de hoy sin hora
            var hoy = DateTimeOffset.Now.Date;
            return fecha >= hoy;
        }
    }
}
