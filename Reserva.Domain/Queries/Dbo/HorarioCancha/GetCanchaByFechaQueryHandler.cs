using AutoMapper;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.HorarioCancha;
using Reserva.Repository.Abstractions.Base;
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

        public GetCanchaByFechaQueryHandler(
            IMapper mapper,
            IRepository<Entity.HorarioCancha> HorarioCanchaRepository,
            IRepository<Entity.Reserva> ReservaRepository
        ) : base(mapper)
        {
            _HorarioCanchaRepository = HorarioCanchaRepository;
            _ReservaRepository = ReservaRepository;
        }

        protected override async Task<ResponseDto<List<HorarioDisponibleDto>>> HandleQuery(GetCanchaByFechaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<List<HorarioDisponibleDto>>
            {
                Data = new List<HorarioDisponibleDto>()
            };

            var diaSemana = request.Fecha.ToString("dddd", new CultureInfo("es-ES")).ToLowerInvariant();

            var horariosDisponibles = await _HorarioCanchaRepository.FindByAsNoTrackingAsync(
                x => x.IdCancha == request.CanchaId && x.IdDiaSemanaNavigation.Nombre.Contains(diaSemana) && x.Activo,
                x => x.IdHoraInicioNavigation
            );

            if (horariosDisponibles == null || !horariosDisponibles.Any())
                return response;

            var reservas = await _ReservaRepository.FindByAsNoTrackingAsync(
                x => x.IdCancha == request.CanchaId
                     && x.FechaReserva.Date == request.Fecha.Date
                     && x.Activo,
                x => x.DetalleReserva
            );

            // Obtener los IDs de horarios ya reservados
            var horariosReservadosIds = reservas
                .SelectMany(r => r.DetalleReserva)
                .Where(d => d.IdHorarioCancha.HasValue && d.Activo)
                .Select(d => d.IdHorarioCancha.Value)
                .ToHashSet();

            var listaHorarios = new List<HorarioDisponibleDto>();

            foreach (var horario in horariosDisponibles)
            {
                if (horariosReservadosIds.Contains(horario.IdHorarioCancha))
                    continue;

                var horarioDto = new HorarioDisponibleDto
                {
                    Hora = horario.IdHoraInicioNavigation.Hora1,
                    HoraTexto = horario.IdHoraInicioNavigation.HoraTexto,
                    Precio = horario.PrecioHora
                };

                listaHorarios.Add(horarioDto);
            }

            // Si la fecha es HOY, eliminar horas pasadas
            var ahora = DateTimeOffset.Now;
            if (request.Fecha.Date == ahora.Date)
            {
                var horaActual = TimeOnly.FromDateTime(ahora.ToLocalTime().DateTime);
                listaHorarios = listaHorarios
                    .Where(h => h.Hora > horaActual)
                    .ToList();
            }

            response.Data = listaHorarios.OrderBy(h => h.Hora).ToList();

            return response;
        }
    }
}
