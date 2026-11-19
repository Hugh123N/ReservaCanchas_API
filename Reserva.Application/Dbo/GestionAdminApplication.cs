using MediatR;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Application.Base;
using Reserva.Domain.Commands.GestionAdmin;
using Reserva.Domain.Queries.GestionAdmin;
using Reserva.Domain.Queries.Dbo.Cancha;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Cancha;
using Reserva.Dto.GestionAdmin;

namespace Reserva.Application.Dbo;

public class GestionAdminApplication(IMediator mediator) : ApplicationBase(mediator), IGestionAdminApplication
{
    public Task<ResponseDto<PaginaGestionAdminDto<ProveedorAdminDto>>> Proveedores(FiltroGestionAdminDto filtro, CancellationToken ct)
        => _mediator.Send(new GetProveedoresAdminQuery(filtro), ct);
    public Task<ResponseDto<ProveedorAdminDto>> Proveedor(int id, CancellationToken ct)
        => _mediator.Send(new GetProveedorAdminQuery(id), ct);
    public Task<ResponseDto<PaginaGestionAdminDto<CanchaAdminDto>>> Canchas(FiltroGestionAdminDto filtro, CancellationToken ct)
        => _mediator.Send(new GetCanchasAdminQuery(filtro), ct);
    public Task<ResponseDto<GetCanchaDto>> Cancha(int id, CancellationToken ct)
        => _mediator.Send(new GetCanchaQuery(id, true), ct);
    public Task<ResponseDto> CambiarProveedor(int id, CambiarEstadoGestionDto cambio, CancellationToken ct)
        => _mediator.Send(new CambiarProveedorAdminCommand(id, cambio), ct);
    public Task<ResponseDto> CambiarCancha(int id, CambiarEstadoGestionDto cambio, CancellationToken ct)
        => _mediator.Send(new CambiarCanchaAdminCommand(id, cambio), ct);
    public Task<ResponseDto<EstadoNegocioDto>> EstadoNegocio(CancellationToken ct)
        => _mediator.Send(new GetEstadoNegocioQuery(), ct);
    public Task<ResponseDto> ReenviarCancha(int id, CambiarEstadoGestionDto cambio, CancellationToken ct)
        => _mediator.Send(new ReenviarCanchaRevisionCommand(id, cambio), ct);
}
