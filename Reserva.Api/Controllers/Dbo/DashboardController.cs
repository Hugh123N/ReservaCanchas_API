using Microsoft.AspNetCore.Mvc;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Dashboard;

namespace Reserva.Api.Controllers.Dbo
{
    [ApiController]
    [Route("api/Dashboard")]
    [Security.Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardApplication _dashboardApplication;

        public DashboardController(IDashboardApplication dashboardApplication)
        {
            _dashboardApplication = dashboardApplication;
        }

        [HttpGet("proveedor")]
        public async Task<ResponseDto<GetDashboardProveedorDto>> GetDashboardProveedor()
            => await _dashboardApplication.GetDashboardProveedor();
    }
}
