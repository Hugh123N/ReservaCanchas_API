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

        [HttpGet("admin")]
        [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme,
            Roles = Common.Constants.Role.Admin)]
        public Task<ResponseDto<GetDashboardAdminDto>> GetDashboardAdmin([FromQuery] int meses = 6, CancellationToken cancellationToken = default)
            => _dashboardApplication.GetDashboardAdmin(meses, cancellationToken);

        [HttpGet("proveedor")]
        public async Task<ResponseDto<GetDashboardProveedorDto>> GetDashboardProveedor()
            => await _dashboardApplication.GetDashboardProveedor();

        [HttpGet("operador")]
        public async Task<ResponseDto<GetDashboardOperadorDto>> GetDashboardOperador()
            => await _dashboardApplication.GetDashboardOperador();
    }
}
