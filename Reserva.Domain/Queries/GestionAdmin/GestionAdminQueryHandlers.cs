using Reserva.Domain.Queries.Base;
using Reserva.Domain.Services.GestionAdmin;
using Reserva.Dto.Base;
using Reserva.Dto.GestionAdmin;

namespace Reserva.Domain.Queries.GestionAdmin;

public class GetProveedoresAdminQueryHandler(GestionAdminService service) : QueryHandlerBase<GetProveedoresAdminQuery, PaginaGestionAdminDto<ProveedorAdminDto>>
{
    protected override Task<ResponseDto<PaginaGestionAdminDto<ProveedorAdminDto>>> HandleQuery(GetProveedoresAdminQuery request, CancellationToken ct)
        => service.ProveedoresAsync(request.Filtro, ct);
}

public class GetProveedorAdminQueryHandler(GestionAdminService service) : QueryHandlerBase<GetProveedorAdminQuery, ProveedorAdminDto>
{
    protected override Task<ResponseDto<ProveedorAdminDto>> HandleQuery(GetProveedorAdminQuery request, CancellationToken ct)
        => service.ProveedorAsync(request.Id, ct);
}

public class GetCanchasAdminQueryHandler(GestionAdminService service) : QueryHandlerBase<GetCanchasAdminQuery, PaginaGestionAdminDto<CanchaAdminDto>>
{
    protected override Task<ResponseDto<PaginaGestionAdminDto<CanchaAdminDto>>> HandleQuery(GetCanchasAdminQuery request, CancellationToken ct)
        => service.CanchasAsync(request.Filtro, ct);
}

public class GetEstadoNegocioQuery : QueryBase<EstadoNegocioDto> { }

public class GetEstadoNegocioQueryHandler(NegocioAccessService access) : QueryHandlerBase<GetEstadoNegocioQuery, EstadoNegocioDto>
{
    protected override async Task<ResponseDto<EstadoNegocioDto>> HandleQuery(GetEstadoNegocioQuery request, CancellationToken ct)
        => new(await access.EstadoActualAsync());
}
