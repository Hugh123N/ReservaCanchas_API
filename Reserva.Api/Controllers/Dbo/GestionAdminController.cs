using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Common;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Cancha;
using Reserva.Dto.GestionAdmin;

namespace Reserva.Api.Controllers.Dbo;

[ApiController]
[Route("api/GestionAdmin")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.Role.Admin)]
public class GestionAdminController(IGestionAdminApplication application) : ControllerBase
{
    [HttpGet("proveedores")]
    public Task<ResponseDto<PaginaGestionAdminDto<ProveedorAdminDto>>> Proveedores([FromQuery] FiltroGestionAdminDto filtro, CancellationToken ct)
        => application.Proveedores(filtro, ct);
    [HttpGet("proveedores/{id:int}")]
    public Task<ResponseDto<ProveedorAdminDto>> Proveedor(int id, CancellationToken ct) => application.Proveedor(id, ct);
    [HttpPut("proveedores/{id:int}/estado")]
    public Task<ResponseDto> CambiarProveedor(int id, CambiarEstadoGestionDto cambio, CancellationToken ct)
        => application.CambiarProveedor(id, cambio, ct);
    [HttpGet("canchas")]
    public Task<ResponseDto<PaginaGestionAdminDto<CanchaAdminDto>>> Canchas([FromQuery] FiltroGestionAdminDto filtro, CancellationToken ct)
        => application.Canchas(filtro, ct);
    [HttpGet("canchas/{id:int}")]
    public Task<ResponseDto<GetCanchaDto>> Cancha(int id, CancellationToken ct) => application.Cancha(id, ct);
    [HttpPut("canchas/{id:int}/estado")]
    public Task<ResponseDto> CambiarCancha(int id, CambiarEstadoGestionDto cambio, CancellationToken ct)
        => application.CambiarCancha(id, cambio, ct);
}

[ApiController]
[Route("api/GestionNegocio")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.Role.Proveedor + "," + Constants.Role.Operador)]
public class GestionNegocioController(IGestionAdminApplication application) : ControllerBase
{
    [HttpGet("estado")]
    public Task<ResponseDto<EstadoNegocioDto>> Estado(CancellationToken ct) => application.EstadoNegocio(ct);
    [HttpPost("canchas/{id:int}/reenviar")]
    [Authorize(Roles = Constants.Role.Proveedor)]
    public Task<ResponseDto> ReenviarCancha(int id, CambiarEstadoGestionDto cambio, CancellationToken ct) => application.ReenviarCancha(id, cambio, ct);
}
