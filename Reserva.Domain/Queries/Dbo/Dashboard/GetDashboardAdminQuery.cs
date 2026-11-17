using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Dashboard;

namespace Reserva.Domain.Queries.Dbo.Dashboard;

public class GetDashboardAdminQuery(int meses = 6) : QueryBase<GetDashboardAdminDto>
{
    public int Meses { get; } = meses;
}
