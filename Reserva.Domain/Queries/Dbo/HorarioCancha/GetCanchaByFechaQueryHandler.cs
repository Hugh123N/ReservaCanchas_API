using AutoMapper;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.HorarioCancha;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Domain.Queries.Dbo.HorarioCancha
{
    internal class GetCanchaByFechaQueryHandler : QueryHandlerBase<GetCanchaByFechaQuery, List<HorarioDisponibleDto>>
    {
        private readonly IRepository<Entity.HorarioCancha> _HorarioCanchaRepository;
        private readonly IRepository<Entity.Reserva> _ReservaRepository;
        private readonly IRepository<Entity.Cancha> _CanchaRepository;

        public GetCanchaByFechaQueryHandler(
            IMapper mapper,
            IRepository<Entity.HorarioCancha> HorarioCanchaRepository,
            IRepository<Entity.Reserva> ReservaRepository,
            IRepository<Entity.Cancha> CanchaRepository
        ) : base(mapper)
        {
            _HorarioCanchaRepository = HorarioCanchaRepository;
            _ReservaRepository = ReservaRepository;
            _CanchaRepository = CanchaRepository;
        }

        protected override async Task<ResponseDto<List<HorarioDisponibleDto>>> HandleQuery(GetCanchaByFechaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<List<HorarioDisponibleDto>>
            {
                Data = new List<HorarioDisponibleDto>()
            };

            var cancha = await _CanchaRepository.GetByAsync(c => c.IdCancha == request.CanchaId && c.Activo);
            var zonaHoraria = TimezoneUtils.ObtenerZonaHoraria(cancha!.ZonaHoraria);

            var diaSemana = request.Fecha.ToString("dddd", new CultureInfo("es-ES")).ToLowerInvariant();

            var horariosDisponibles = await _HorarioCanchaRepository.FindByAsNoTrackingAsync(
                x => x.IdCancha == request.CanchaId && x.IdDiaSemanaNavigation.Nombre.Contains(diaSemana) && x.Activo,
                x => x.IdHoraInicioNavigation,
                x => x.IdHoraFinNavigation!
            );

            if (horariosDisponibles == null || !horariosDisponibles.Any())
                return response;

            var fechaBuscadaLocal = DateTimeHelper.NormalizarFechaLocal(request.Fecha, zonaHoraria);

            var reservas = await _ReservaRepository.FindByAsNoTrackingAsync(
                x => x.IdCancha == request.CanchaId
                     && x.Activo,
                x => x.DetalleReserva
            );

            var reservasDelDia = reservas
                .Where(r => DateTimeHelper.NormalizarFechaLocal(r.FechaReserva, zonaHoraria).Date == fechaBuscadaLocal.Date)
                .ToList();

            // Obtener los IDs de horarios ya reservados
            var horariosReservadosIds = reservasDelDia
                .SelectMany(r => r.DetalleReserva)
                .Where(d => d.IdHorarioCancha.HasValue && d.Activo)
                .Select(d => d.IdHorarioCancha!.Value)
                .ToHashSet();

            var listaHorarios = new List<HorarioDisponibleDto>();

            foreach (var horario in horariosDisponibles)
            {
                if (horariosReservadosIds.Contains(horario.IdHorarioCancha))
                    continue;

                var horarioDto = new HorarioDisponibleDto
                {
                    IdHorarioCancha = horario.IdHorarioCancha,
                    HoraInicio = horario.IdHoraInicioNavigation.Hora1,
                    HoraInicioTexto = horario.IdHoraInicioNavigation.HoraTexto,
                    HoraFin = horario.IdHoraFinNavigation?.Hora1 ?? default,
                    HoraFinTexto = horario.IdHoraFinNavigation?.HoraTexto ?? "",
                    Precio = horario.PrecioHora
                };

                listaHorarios.Add(horarioDto);
            }

            // Si la fecha es HOY (en la zona horaria de la cancha), eliminar horas pasadas
            var ahoraLocal = DateTimeHelper.ObtenerAhoraLocal(zonaHoraria);
            if (fechaBuscadaLocal.Date == ahoraLocal.Date)
            {
                var horaActual = TimeOnly.FromDateTime(ahoraLocal.DateTime);
                listaHorarios = listaHorarios
                    .Where(h => h.HoraInicio > horaActual)
                    .ToList();
            }

            response.Data = listaHorarios.OrderBy(h => h.HoraInicio).ToList();

            return response;
        }
    }
}
