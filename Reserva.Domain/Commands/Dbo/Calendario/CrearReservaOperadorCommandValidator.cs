using FluentValidation;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Calendario;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.Calendario
{
    /// <summary>
    /// Validador para CrearReservaOperadorCommand
    /// </summary>
    public class CrearReservaOperadorCommandValidator : CommandValidatorBase<CrearReservaOperadorCommand>
    {
        private readonly IRepository<Entity.AspNetUsers> _userRepository;
        private readonly IRepository<Entity.Cancha> _canchaRepository;

        public CrearReservaOperadorCommandValidator(
            IRepository<Entity.AspNetUsers> userRepository,
            IRepository<Entity.Cancha> canchaRepository)
        {
            _userRepository = userRepository;
            _canchaRepository = canchaRepository;

            ConfigureRules();
        }

        private void ConfigureRules()
        {
            RequiredInformation(x => x.RequestDto).DependentRules(() =>
            {
                // Validar Cancha
                RuleFor(x => x.RequestDto.IdCancha)
                    .GreaterThan(0)
                    .WithMessage("Debe seleccionar una cancha")
                    .MustAsync(async (idCancha, cancellationToken) =>
                    {
                        var cancha = await _canchaRepository.GetByAsync(
                            c => c.IdCancha == idCancha && c.Activo);
                        return cancha != null;
                    })
                    .WithMessage("La cancha seleccionada no existe o no está activa");

                // Validar Tipo de Deporte
                RuleFor(x => x.RequestDto.IdTipoDeporte)
                    .GreaterThan(0)
                    .WithMessage("Debe seleccionar un tipo de deporte");

                // Validar Cliente
                RuleFor(x => x.RequestDto.Cliente)
                    .NotNull()
                    .WithMessage("Debe proporcionar información del cliente");

                When(x => x.RequestDto.Cliente != null, () =>
                {
                    // Si es cliente existente
                    When(x => !x.RequestDto.Cliente.EsNuevoCliente, () =>
                    {
                        RuleFor(x => x.RequestDto.Cliente.IdCliente)
                            .NotNull()
                            .NotEmpty()
                            .WithMessage("Debe proporcionar el ID del cliente")
                            .MustAsync(async (idCliente, cancellationToken) =>
                            {
                                if (!idCliente.HasValue) return false;
                                var user = await _userRepository.GetByAsync(
                                    u => u.Id == idCliente.Value && u.Activo);
                                return user != null;
                            })
                            .WithMessage("El cliente seleccionado no existe");
                    });

                    // Si es cliente nuevo
                    When(x => x.RequestDto.Cliente.EsNuevoCliente, () =>
                    {
                        RuleFor(x => x.RequestDto.Cliente.NombreCompleto)
                            .NotEmpty()
                            .WithMessage("El nombre del cliente es obligatorio")
                            .MinimumLength(3)
                            .WithMessage("El nombre debe tener al menos 3 caracteres");

                        RuleFor(x => x.RequestDto.Cliente.Telefono)
                            .NotEmpty()
                            .WithMessage("El teléfono del cliente es obligatorio")
                            .Matches(@"^\d{7,15}$")
                            .WithMessage("El formato del teléfono no es válido")
                            .MustAsync(async (telefono, cancellationToken) =>
                            {
                                if (string.IsNullOrEmpty(telefono)) return true;
                                var user = await _userRepository.GetByAsync(
                                    u => u.PhoneNumber == telefono && u.Activo);
                                return user == null;
                            })
                            .WithMessage("El teléfono ya está registrado");

                        RuleFor(x => x.RequestDto.Cliente.Email)
                            .EmailAddress()
                            .When(x => !string.IsNullOrEmpty(x.RequestDto.Cliente.Email))
                            .WithMessage("El formato del email no es válido")
                            .MustAsync(async (email, cancellationToken) =>
                            {
                                if (string.IsNullOrEmpty(email)) return true;
                                var user = await _userRepository.GetByAsync(
                                    u => u.Email == email && u.Activo);
                                return user == null;
                            })
                            .WithMessage("El email ya está registrado");
                    });
                });

                // Validar Horarios
                RuleFor(x => x.RequestDto.Horarios)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Debe seleccionar al menos un horario");

                When(x => x.RequestDto.Horarios != null && x.RequestDto.Horarios.Any(), () =>
                {
                    RuleForEach(x => x.RequestDto.Horarios).ChildRules(bloque =>
                    {
                        bloque.RuleFor(h => h.Fecha)
                            .NotEmpty()
                            .WithMessage("La fecha es obligatoria")
                            .GreaterThanOrEqualTo(DateTimeOffset.UtcNow.Date)
                            .WithMessage("La fecha no puede ser anterior a hoy");

                        bloque.RuleFor(h => h.IdHorarioCanchaInicio)
                            .GreaterThan(0)
                            .WithMessage("El horario de inicio es obligatorio");

                        bloque.RuleFor(h => h.IdHorarioCanchaFin)
                            .GreaterThan(0)
                            .WithMessage("El horario de fin es obligatorio")
                            .Must((bloque, idHorarioCanchaFin) => idHorarioCanchaFin >= bloque.IdHorarioCanchaInicio)
                            .WithMessage("El horario de fin debe ser mayor que el horario de inicio");
                    });
                });

                // Validar Pago
                RuleFor(x => x.RequestDto.Pago)
                    .NotNull()
                    .WithMessage("Debe proporcionar información del pago");

                When(x => x.RequestDto.Pago != null, () =>
                {
                    RuleFor(x => x.RequestDto.Pago.MontoTotal)
                        .GreaterThan(0)
                        .WithMessage("El monto total debe ser mayor a cero");

                    // Si es reserva inmediata, validar monto pagado
                    When(x => x.RequestDto.TipoReserva == TipoReservaOperador.Inmediata, () =>
                    {
                        RuleFor(x => x.RequestDto.Pago.MontoPagado)
                            .NotNull()
                            .GreaterThan(0)
                            .WithMessage("Para reserva inmediata, el monto pagado debe ser mayor a cero");

                        RuleFor(x => x.RequestDto.Pago.CodigoMetodoPago)
                            .NotEmpty()
                            .WithMessage("Debe seleccionar un método de pago");
                    });
                });
            });

        }
    }
}
