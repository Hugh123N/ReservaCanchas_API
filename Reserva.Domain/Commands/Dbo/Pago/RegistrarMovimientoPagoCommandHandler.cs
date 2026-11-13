using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Services.Pago;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Pago;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.Pago;

public class RegistrarMovimientoPagoCommandHandler : CommandHandlerBase<RegistrarMovimientoPagoCommand, GetHistorialPagoDto>
{
    private readonly MovimientoPagoService _service;
    private readonly ILogger<RegistrarMovimientoPagoCommandHandler> _logger;

    public RegistrarMovimientoPagoCommandHandler(IUnitOfWork unitOfWork, IMapper mapper,
        RegistrarMovimientoPagoCommandValidator validator, MovimientoPagoService service,
        ILogger<RegistrarMovimientoPagoCommandHandler> logger) : base(unitOfWork, mapper, validator)
    {
        _service = service;
        _logger = logger;
    }

    public override Task<ResponseDto<GetHistorialPagoDto>> HandleCommand(RegistrarMovimientoPagoCommand request, CancellationToken cancellationToken)
        => _service.RegistrarAsync(request.Dto, request.Correccion);

    public override Task<ResponseDto<GetHistorialPagoDto>?> OnHandleError(Exception ex)
    {
        if (ex is not DbUpdateException)
            return base.OnHandleError(ex);

        var response = new ResponseDto<GetHistorialPagoDto>();
        _logger.LogError(ex, "No se pudo persistir el cobro o la corrección de una reserva.");
        response.AddErrorResult("No se pudo guardar la operación. No se aplicaron cambios. Revisa el detalle antes de reintentar.");
        return Task.FromResult<ResponseDto<GetHistorialPagoDto>?>(response);
    }
}
