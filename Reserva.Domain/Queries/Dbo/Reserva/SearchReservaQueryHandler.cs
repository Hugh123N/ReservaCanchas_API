using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Domain.Services.Reporte;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Reserva;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.Reserva;

public class SearchReservaQueryHandler : SearchQueryHandlerBase<SearchReservaQuery, SearchReservaFilterDto, ReservaClienteDto, SearchReservasResultDto>
{
    private readonly IRepository<Entity.Reserva> _reservas;
    private readonly IRepository<Entity.DetalleReserva> _detalles;
    private readonly IRepository<Entity.Pago> _pagos;
    private readonly IRepository<Entity.MovimientoPago> _movimientos;
    private readonly ReporteAccessService _access;

    public SearchReservaQueryHandler(IMapper mapper, SearchReservaQueryValidator validator, IRepository<Entity.Reserva> reservas,
        IRepository<Entity.DetalleReserva> detalles, IRepository<Entity.Pago> pagos, IRepository<Entity.MovimientoPago> movimientos,
        ReporteAccessService access) : base(mapper, validator)
    { _reservas = reservas; _detalles = detalles; _pagos = pagos; _movimientos = movimientos; _access = access; }

    protected override async Task<ResponseDto<SearchReservasResultDto>> HandleQuery(SearchReservaQuery request, CancellationToken cancellationToken)
    {
        var response = new ResponseDto<SearchReservasResultDto>();
        var filtro = request.SearchParams.Filter ?? new();
        var canchas = await _access.ObtenerCanchasAsync();
        if (canchas == null || (filtro.IdCancha.HasValue && !canchas.Any(c => c.IdCancha == filtro.IdCancha)))
        { response.AddErrorResult("No tienes acceso a las canchas solicitadas."); return response; }
        var pagina = request.SearchParams.Page!.Page;
        var tamano = request.SearchParams.Page.PageSize;
        if (canchas.Count == 0)
        {
            response.UpdateData(new SearchReservasResultDto(Array.Empty<ReservaClienteDto>(), new(), pagina, tamano));
            return response;
        }
        var query = SearchReservaConsultas.Filtrar(_reservas.FindAllAsNoTracking(), _pagos.FindAllAsNoTracking(), canchas, filtro);
        var indicadores = await SearchReservaConsultas.Indicadores(query).SingleOrDefaultAsync(cancellationToken) ?? new();
        var filas = await SearchReservaConsultas.Ordenar(query, request.SearchParams.Sort).Skip((pagina - 1) * tamano).Take(tamano).ToListAsync(cancellationToken);
        if (filas.Count > 0)
        {
            var ids = filas.Select(r => r.IdReserva).ToList();
            var detalles = await _detalles.FindByAsNoTrackingAsync(d => d.Activo && ids.Contains(d.IdReserva),
                d => d.IdHorarioCanchaNavigation!.IdHoraInicioNavigation, d => d.IdHorarioCanchaNavigation!.IdHoraFinNavigation!);
            var reembolsos = await MovimientosReporteConsultas.Proyectar(_movimientos.FindAllAsNoTracking().Where(m => m.Activo &&
                m.Tipo == "REEMBOLSO" && m.IdPagoNavigation.IdReserva.HasValue && ids.Contains(m.IdPagoNavigation.IdReserva.Value))).ToListAsync(cancellationToken);
            SearchReservaPaginaMapper.Completar(filas, detalles, reembolsos, canchas);
        }
        response.UpdateData(new SearchReservasResultDto(filas, indicadores, pagina, tamano));
        return response;
    }
}
