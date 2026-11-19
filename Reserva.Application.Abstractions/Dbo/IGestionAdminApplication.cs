using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Cancha;
using Reserva.Dto.GestionAdmin;

namespace Reserva.Application.Abstractions.Dbo;

public interface IGestionAdminApplication
{
    Task<ResponseDto<PaginaGestionAdminDto<ProveedorAdminDto>>> Proveedores(FiltroGestionAdminDto filtro, CancellationToken ct);
    Task<ResponseDto<ProveedorAdminDto>> Proveedor(int id, CancellationToken ct);
    Task<ResponseDto<PaginaGestionAdminDto<CanchaAdminDto>>> Canchas(FiltroGestionAdminDto filtro, CancellationToken ct);
    Task<ResponseDto<GetCanchaDto>> Cancha(int id, CancellationToken ct);
    Task<ResponseDto> CambiarProveedor(int id, CambiarEstadoGestionDto cambio, CancellationToken ct);
    Task<ResponseDto> CambiarCancha(int id, CambiarEstadoGestionDto cambio, CancellationToken ct);
    Task<ResponseDto<EstadoNegocioDto>> EstadoNegocio(CancellationToken ct);
    Task<ResponseDto> ReenviarCancha(int id, CambiarEstadoGestionDto cambio, CancellationToken ct);
}
