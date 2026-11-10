using Moq;
using Reserva.Domain.Queries.Dbo.Calendario;
using Reserva.Domain.Services.PersonLookup;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.PersonLookup;
using Reserva.Entity;
using Reserva.Repository.Abstractions.Base;
using System.Linq.Expressions;
using Xunit;

namespace Reserva.Domain.Tests.Calendario;

public class BuscarClienteQueryTests
{
    private const string Dni = "00000001";
    private readonly Mock<IRepository<Cliente>> _repository = new(MockBehavior.Strict);
    private readonly Mock<IPersonLookupService> _lookup = new(MockBehavior.Strict);

    [Fact]
    public async Task ExistingDni_UsesLocalRecord_WithoutCallingPersonLookup()
    {
        var local = new Cliente
        {
            IdCliente = 7, TipoDoc = "DNI", NroDoc = Dni, Nombres = "NOMBRES",
            Apellidos = "PATERNO MATERNO", Telefono = "999888777", Activo = true
        };
        Expression<Func<Cliente, bool>>? filter = null;
        _repository.Setup(repository => repository.GetByAsNoTrackingAsync(It.IsAny<Expression<Func<Cliente, bool>>>()))
            .Callback<Expression<Func<Cliente, bool>>>(predicate => filter = predicate)
            .ReturnsAsync(local);

        var response = await CreateHandler().Handle(new BuscarClienteQuery { Dni = Dni }, CancellationToken.None);

        Assert.True(response.IsValid);
        Assert.Equal(7, response.Data!.IdCliente);
        Assert.False(response.Data.EsNuevoCliente);
        Assert.Equal("999888777", response.Data.Telefono);
        Assert.Equal("NOMBRES PATERNO MATERNO", response.Data.NombreCompleto);
        Assert.True(filter!.Compile()(local));
        Assert.False(filter.Compile()(new Cliente { TipoDoc = "RUC", NroDoc = Dni, Activo = true }));
        Assert.False(filter.Compile()(new Cliente { TipoDoc = "DNI", NroDoc = "00000002", Activo = true }));
        Assert.False(filter.Compile()(new Cliente { TipoDoc = "DNI", NroDoc = Dni, Activo = false }));
        _lookup.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData("MATERNO", "PATERNO MATERNO")]
    [InlineData(null, "PATERNO")]
    public async Task UnknownDni_ReturnsPersonalDataWithoutRegisteringClient(string? maternalSurname, string expectedSurnames)
    {
        _repository.Setup(repository => repository.GetByAsNoTrackingAsync(It.IsAny<Expression<Func<Cliente, bool>>>()))
            .ReturnsAsync((Cliente?)null);
        using var cancellation = new CancellationTokenSource();
        _lookup.Setup(service => service.GetDniAsync(Dni, cancellation.Token))
            .ReturnsAsync(new ResponseDto<DniLookupDto>(new DniLookupDto
            {
                FirstName = "NOMBRES", LastName = "PATERNO", MaternalLastName = maternalSurname, DocumentNumber = Dni
            }));

        var response = await CreateHandler().Handle(new BuscarClienteQuery { Dni = Dni }, cancellation.Token);

        Assert.True(response.IsValid);
        Assert.Null(response.Data!.IdCliente);
        Assert.True(response.Data.EsNuevoCliente);
        Assert.Equal(Dni, response.Data.NroDoc);
        Assert.Equal("DNI", response.Data.TipoDoc);
        Assert.Equal(expectedSurnames, response.Data.LastName);
        Assert.Null(response.Data.Telefono);
        _repository.VerifyAll();
        _repository.VerifyNoOtherCalls();
        _lookup.VerifyAll();
    }

    [Fact]
    public async Task LookupFailure_PropagatesError_AndDoesNotReturnAnEmptyClient()
    {
        _repository.Setup(repository => repository.GetByAsNoTrackingAsync(It.IsAny<Expression<Func<Cliente, bool>>>()))
            .ReturnsAsync((Cliente?)null);
        var failure = new ResponseDto<DniLookupDto>();
        failure.AddErrorResult("No se pudo consultar el DNI.");
        _lookup.Setup(service => service.GetDniAsync(Dni, It.IsAny<CancellationToken>())).ReturnsAsync(failure);

        var response = await CreateHandler().Handle(new BuscarClienteQuery { Dni = Dni }, CancellationToken.None);

        Assert.False(response.IsValid);
        Assert.Null(response.Data);
        Assert.Contains(response.Messages, item => item.Message == "No se pudo consultar el DNI.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("1234567")]
    [InlineData("123456789")]
    [InlineData("1234567A")]
    [InlineData("１２３４５６７８")]
    public async Task InvalidDni_DoesNotQueryDatabaseOrProvider(string dni)
    {
        var response = await CreateHandler().Handle(new BuscarClienteQuery { Dni = dni }, CancellationToken.None);

        Assert.False(response.IsValid);
        Assert.Null(response.Data);
        _repository.VerifyNoOtherCalls();
        _lookup.VerifyNoOtherCalls();
    }

    private BuscarClienteQueryHandler CreateHandler() => new(_repository.Object, _lookup.Object);
}
