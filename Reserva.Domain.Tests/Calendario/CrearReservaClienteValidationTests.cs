using Moq;
using Reserva.Domain.Commands.Dbo.Calendario;
using Reserva.Dto.Dbo.Calendario;
using Reserva.Entity;
using Reserva.Repository.Abstractions.Base;
using System.Linq.Expressions;
using Xunit;

namespace Reserva.Domain.Tests.Calendario;

public class CrearReservaClienteValidationTests
{
    [Theory]
    [InlineData(true, "")]
    [InlineData(false, "")]
    [InlineData(true, "1234567")]
    [InlineData(false, "1234567")]
    [InlineData(true, "99988877A")]
    [InlineData(false, "99988877A")]
    public async Task PhoneIsRequiredAndNumeric_ForBothNewAndExistingClients(bool isNew, string phone)
    {
        var (validator, command) = CreateScenario(isNew, phone);

        var result = await validator.ValidateAsync(command);

        Assert.Contains(result.Errors, error => error.PropertyName == "RequestDto.Cliente.Telefono");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ValidPhoneAndDni_AreAccepted(bool isNew)
    {
        var (validator, command) = CreateScenario(isNew, "999888777");

        var result = await validator.ValidateAsync(command);

        Assert.True(result.IsValid, string.Join("; ", result.Errors.Select(error => error.ErrorMessage)));
    }

    [Fact]
    public async Task ExistingClientIdMustMatchTheDni()
    {
        var (validator, command) = CreateScenario(false, "999888777");
        command.RequestDto.Cliente.NroDoc = "00000002";

        var result = await validator.ValidateAsync(command);

        Assert.Contains(result.Errors, error => error.PropertyName == "RequestDto.Cliente.IdCliente");
    }

    private static (CrearReservaOperadorCommandValidator Validator, CrearReservaOperadorCommand Command) CreateScenario(bool isNew, string phone)
    {
        var clients = new[]
        {
            new Cliente { IdCliente = 7, Activo = true, TipoDoc = "DNI", NroDoc = "00000001", Telefono = "999888777" }
        };
        var repository = new Mock<IRepository<Cliente>>();
        repository.Setup(item => item.GetByAsync(It.IsAny<Expression<Func<Cliente, bool>>>()))
            .Returns((Expression<Func<Cliente, bool>> predicate) => Task.FromResult(clients.FirstOrDefault(predicate.Compile())));
        var canchas = new Mock<IRepository<Cancha>>();
        canchas.Setup(item => item.GetByAsync(It.IsAny<Expression<Func<Cancha, bool>>>()))
            .ReturnsAsync(new Cancha { IdCancha = 1, Activo = true });

        var command = new CrearReservaOperadorCommand
        {
            RequestDto = new CrearReservaOperadorRequestDto
            {
                IdCancha = 1, IdTipoDeporte = 1, TipoReserva = TipoReservaOperador.Normal,
                Cliente = new ClienteReservaDto
                {
                    IdCliente = isNew ? null : 7, EsNuevoCliente = isNew, TipoDoc = "DNI", NroDoc = "00000001",
                    Nombre = "NOMBRES", Apellidos = "PATERNO MATERNO", Telefono = phone
                },
                Horarios = new List<BloqueHorarioDto>
                {
                    new() { Fecha = DateTimeOffset.UtcNow, IdHorarioCanchaInicio = 1, IdHorarioCanchaFin = 2 }
                },
                Pago = new PagoReservaOperadorDto { MontoTotal = 100, CodigoMetodoPago = "02" }
            }
        };
        return (new CrearReservaOperadorCommandValidator(repository.Object, canchas.Object), command);
    }
}
