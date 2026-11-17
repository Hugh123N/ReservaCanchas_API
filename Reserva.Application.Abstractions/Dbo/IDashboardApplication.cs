using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Dashboard;

namespace Reserva.Application.Abstractions.Dbo
{
    public interface IDashboardApplication
    {
        Task<ResponseDto<GetDashboardAdminDto>> GetDashboardAdmin(int meses, CancellationToken cancellationToken);
        Task<ResponseDto<GetDashboardProveedorDto>> GetDashboardProveedor();
        Task<ResponseDto<GetDashboardOperadorDto>> GetDashboardOperador();
    }
}
