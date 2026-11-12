using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Services.Pago;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Pago;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.Pago;

public class RegistrarMovimientoPagoCommandHandler : CommandHandlerBase<RegistrarMovimientoPagoCommand, GetHistorialPagoDto>
{
    private readonly MovimientoPagoService _service;

    public RegistrarMovimientoPagoCommandHandler(IUnitOfWork unitOfWork, IMapper mapper,
        RegistrarMovimientoPagoCommandValidator validator, MovimientoPagoService service) : base(unitOfWork, mapper, validator)
        => _service = service;

    public override Task<ResponseDto<GetHistorialPagoDto>> HandleCommand(RegistrarMovimientoPagoCommand request,
        CancellationToken cancellationToken) => _service.RegistrarAsync(request.Dto, request.Correccion);

    public override Task<ResponseDto<GetHistorialPagoDto>?> OnHandleError(Exception ex)
    {
        if (ex is not DbUpdateException) return base.OnHandleError(ex);
        var response = new ResponseDto<GetHistorialPagoDto>();
        response.AddErrorResult("No se registró la operación por un conflicto de datos. Actualiza el historial y verifica el saldo antes de reintentar.");
        return Task.FromResult<ResponseDto<GetHistorialPagoDto>?>(response);
    }
}
