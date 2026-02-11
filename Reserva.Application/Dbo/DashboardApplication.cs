using MediatR;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Application.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Dashboard;
using Reserva.Domain.Queries.Dbo.Dashboard;

namespace Reserva.Application.Dbo
{
    public class DashboardApplication : ApplicationBase, IDashboardApplication
    {
        public DashboardApplication(IMediator mediator) : base(mediator) { }

        public async Task<ResponseDto<GetDashboardProveedorDto>> GetDashboardProveedor()
            => await _mediator.Send(new GetDashboardProveedorQuery());

        public async Task<ResponseDto<GetDashboardOperadorDto>> GetDashboardOperador()
            => await _mediator.Send(new GetDashboardOperadorQuery());
    }
}
