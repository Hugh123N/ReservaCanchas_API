using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Reserva;
using Reserva.Entity.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Dbo.Reserva
{
    /// <summary>
    /// Handler para buscar reservas del cliente con paginación y filtros
    /// </summary>
    public class SearchReservasClienteQueryHandler : SearchQueryHandlerBase<SearchReservasClienteQuery, SearchReservaClienteFilterDto, ReservaClienteDto>
    {
        private readonly IRepository<Entity.Reserva> _reservaRepository;

        public SearchReservasClienteQueryHandler(
            IMapper mapper,
            IRepository<Entity.Reserva> reservaRepository
        ) : base(mapper)
        {
            _reservaRepository = reservaRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<ReservaClienteDto>>> HandleQuery(
            SearchReservasClienteQuery request,
            CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<ReservaClienteDto>>();

            try
            {
                Expression<Func<Entity.Reserva, bool>> filter = x => x.IdCliente == request.IdUsuario && x.Activo;

                var filters = request.SearchParams?.Filter;

                if (filters != null)
                {
                    if (!string.IsNullOrWhiteSpace(filters.CodigoEstado))
                    {
                        filter = filter.And(x => x.IdEstadoReservaNavigation.Codigo == filters.CodigoEstado);
                    }

                    if (filters.FechaDesde.HasValue)
                    {
                        filter = filter.And(x => x.FechaReserva >= filters.FechaDesde.Value);
                    }

                    if (filters.FechaHasta.HasValue)
                    {
                        filter = filter.And(x => x.FechaReserva <= filters.FechaHasta.Value);
                    }

                    if (!string.IsNullOrWhiteSpace(filters.CodigoReserva))
                    {
                        filter = filter.And(x => x.CodigoReserva.Contains(filters.CodigoReserva));
                    }

                    if (!string.IsNullOrWhiteSpace(filters.NombreCancha))
                    {
                        filter = filter.And(x => x.IdCanchaNavigation.Nombre.Contains(filters.NombreCancha));
                    }

                    if (!string.IsNullOrWhiteSpace(filters.EstadoPago))
                    {
                        filter = filter.And(x => x.Pago.Any(p => p.Activo && p.IdEstadoPagoNavigation.Nombre == filters.EstadoPago));
                    }
                }

                var sorts = new List<SortExpression<Entity.Reserva>>();

                if (request.SearchParams?.Sort != null && request.SearchParams.Sort.Any())
                {
                    foreach (var srt in request.SearchParams.Sort)
                    {
                        var property = IQueryableExtensions.GetSortExpression<Entity.Reserva>(srt.Direction, srt.Property);
                        if (property != null) sorts.Add(property);
                    }
                }

                var reservasPaginadas = await _reservaRepository.SearchByAsNoTrackingAsync(
                    request.SearchParams?.Page?.Page ?? 1,
                    request.SearchParams?.Page?.PageSize ?? 10,
                    sorts,
                    filter,
                    r => r.IdCanchaNavigation,
                    r => r.IdEstadoReservaNavigation,
                    r => r.DetalleReserva,
                    r => r.Pago
                );

                // Mapear a DTOs
                var reservasDtos = reservasPaginadas.Items.Select(r => new ReservaClienteDto
                {
                    IdReserva = r.IdReserva,
                    CodigoReserva = r.CodigoReserva,
                    Fecha = r.FechaReserva,
                    Monto = r.MontoTotal,

                    // Estado
                    EstadoReserva = r.IdEstadoReservaNavigation.Nombre,
                    CodigoEstadoReserva = r.IdEstadoReservaNavigation.Codigo!,

                    // Cancha
                    IdCancha = r.IdCancha,
                    NombreCancha = r.IdCanchaNavigation.Nombre,
                    DireccionCancha = r.IdCanchaNavigation.Direccion,
                    TelefonoCancha = r.IdCanchaNavigation.TelefonoCancha,

                    // Horarios
                    Horarios = r.DetalleReserva
                        .OrderBy(d => d.HoraInicio)
                        .Select(d => new HorarioReservadoDto
                        {
                            HoraInicio = d.HoraInicio,
                            HoraFin = d.HoraFin
                        })
                        .ToList(),

                    // Pago
                    EstadoPago = r.Pago.FirstOrDefault(p => p.Activo)?.IdEstadoPagoNavigation?.Nombre ?? "Desconocido",
                    MontoAdelanto = r.Pago.FirstOrDefault(p => p.Activo)?.MontoAdelanto ?? 0,
                    MontoPendiente = r.Pago.FirstOrDefault(p => p.Activo)?.MontoPendiente ?? 0,
                    NumeroRecibo = r.Pago.FirstOrDefault(p => p.Activo)?.NumeroReferencia,

                    // Fechas
                    FechaExpiracionPreReserva = r.FechaExpiracionPreReserva,
                    FechaCreacion = r.CreateDate
                }).ToList();

                var searchResult = new SearchResultDto<ReservaClienteDto>(
                    reservasDtos,
                    reservasPaginadas.Total,
                    request.SearchParams
                );

                response.UpdateData(searchResult);
                response.AddOkResult($"Se encontraron {reservasPaginadas.Total} reservas.");
            }
            catch (Exception ex)
            {
                response.AddErrorResult($"Error al buscar las reservas: {ex.Message}");
            }

            return response;
        }
    }
}
