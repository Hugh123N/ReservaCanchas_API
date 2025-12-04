using AutoMapper;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Domain.Queries.Dbo.HorarioCancha
{
    internal class GetCanchaByFechaQueryHandler : QueryHandlerBase<GetCanchaByFechaQuery, List<string>>
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

        protected override async Task<ResponseDto<List<string>>> HandleQuery(GetCanchaByFechaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<List<string>>
            {
                Data = new List<string>()
            };

            var diaSemana = request.Fecha.ToString("dddd", new CultureInfo("es-ES")).ToLowerInvariant();

            var horariosDisponibles = await _HorarioCanchaRepository.FindByAsNoTrackingAsync(x => x.IdCancha == request.CanchaId && x.IdDiaSemanaNavigation.Nombre.Contains(diaSemana));

            if (horariosDisponibles == null)
                return response;

            foreach (var item in horariosDisponibles)
            {
                if (item.HoraInicio != null && item.HoraFin != null)
                {
                    var horaInicio = item.HoraInicio;
                    var horaFin = item.HoraFin;
                    for (var hora = horaInicio; hora < horaFin; hora = hora.AddHours(1))
                    {

                        response.Data.Add(hora.ToString("HH:mm"));
                    }
                }
            }

            var reservas = await _ReservaRepository.FindByAsNoTrackingAsync(x => x.IdCancha == request.CanchaId
                     && x.FechaReserva.Date == request.Fecha.Date && x.Activo,
                x => x.DetalleReserva
            );

            var horasReservadas = reservas.SelectMany(r => r.DetalleReserva)
                .Select(d => d.HoraInicio.ToString("HH:mm")).ToList();

            // Eliminar las horas que ya están reservadas
            response.Data = response.Data.Except(horasReservadas).ToList();

            // Si la fecha es HOY, eliminar horas pasadas
            var ahora = DateTimeOffset.Now;
            if (request.Fecha.Date == ahora.Date)
            {
                response.Data = response.Data
                    .Where(horaStr =>
                    {
                        var hora = DateTime.ParseExact(horaStr, "HH:mm", CultureInfo.InvariantCulture);
                        return hora > ahora.ToLocalTime();
                    }).ToList();
            }

            return response;
        }
    }
}
