using AutoMapper;
using Reserva.Domain.Queries.Base;
using Reserva.Domain.Services.Pago;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Pago;

namespace Reserva.Domain.Queries.Dbo.Pago;

public class GetHistorialPagoQuery(int idPago, int? idReserva = null) : QueryBase<GetHistorialPagoDto>
{
    public int IdPago { get; } = idPago;
    public int? IdReserva { get; } = idReserva;
}

public class GetHistorialPagoQueryHandler(IMapper mapper, MovimientoPagoService service)
    : QueryHandlerBase<GetHistorialPagoQuery, GetHistorialPagoDto>(mapper)
{
    protected override Task<ResponseDto<GetHistorialPagoDto>> HandleQuery(GetHistorialPagoQuery request,
        CancellationToken cancellationToken) => request.IdReserva.HasValue
            ? service.ObtenerPorReservaAsync(request.IdReserva.Value) : service.ObtenerAsync(request.IdPago);
}
