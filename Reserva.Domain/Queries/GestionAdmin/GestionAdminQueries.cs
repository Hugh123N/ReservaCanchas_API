using Reserva.Domain.Queries.Base;
using Reserva.Dto.GestionAdmin;

namespace Reserva.Domain.Queries.GestionAdmin;

public class GetProveedoresAdminQuery(FiltroGestionAdminDto filtro) : QueryBase<PaginaGestionAdminDto<ProveedorAdminDto>>
{
    public FiltroGestionAdminDto Filtro { get; } = filtro;
}

public class GetProveedorAdminQuery(int id) : QueryBase<ProveedorAdminDto>
{
    public int Id { get; } = id;
}

public class GetCanchasAdminQuery(FiltroGestionAdminDto filtro) : QueryBase<PaginaGestionAdminDto<CanchaAdminDto>>
{
    public FiltroGestionAdminDto Filtro { get; } = filtro;
}
