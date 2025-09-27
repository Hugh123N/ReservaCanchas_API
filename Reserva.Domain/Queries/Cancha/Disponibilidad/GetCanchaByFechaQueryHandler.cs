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

namespace Reserva.Domain.Queries.Cancha.Disponibilidad
{
    internal class GetCanchaByFechaQueryHandler : QueryHandlerBase<GetCanchaByFechaQuery, List<string>>
    {
        private readonly IRepository<Entity.Disponibilidad> _DisponibilidadRepository;
        private readonly IRepository<Entity.Reserva> _ReservaRepository;

        public GetCanchaByFechaQueryHandler(
            IMapper mapper,
            IRepository<Entity.Disponibilidad> DisponibilidadRepository,
            IRepository<Entity.Reserva> ReservaRepository
        ) : base(mapper)
        {
            _DisponibilidadRepository = DisponibilidadRepository;
            _ReservaRepository = ReservaRepository;
        }

        protected override async Task<ResponseDto<List<string>>> HandleQuery(GetCanchaByFechaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<List<string>>
            {
                Data = new List<string>()
            };

            var diaSemana = request.Fecha.ToString("dddd", new CultureInfo("es-ES")).ToLowerInvariant();

            var disponibilidad = await _DisponibilidadRepository.FindByAsNoTrackingAsync(x => x.IdCancha == request.CanchaId && x.IdDiaSemanaNavigation.Nombre.Contains(diaSemana));

            if (disponibilidad == null)
                return response;

            foreach (var item in disponibilidad)
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

            var requestFechaDateOnly = DateOnly.FromDateTime(request.Fecha);
            var reservas = await _ReservaRepository.FindByAsNoTrackingAsync(x => x.IdCancha == request.CanchaId && x.Fecha == requestFechaDateOnly && x.Activo);
            var reservasHoras = reservas.Select(x => x.HoraInicio.ToString("HH:mm")).ToList();

            // Eliminar las horas que ya están reservadas
            response.Data = response.Data.Except(reservasHoras).ToList();

            // Si la fecha es HOY, eliminar horas pasadas
            var hoy = DateOnly.FromDateTime(DateTime.Now);
            if (requestFechaDateOnly == hoy)
            {
                var horaActual = DateTime.Now;

                response.Data = response.Data
                    .Where(horaStr =>
                    {
                        var hora = DateTime.ParseExact(horaStr, "HH:mm", CultureInfo.InvariantCulture);
                        return hora > horaActual;
                    })
                    .ToList();
            }

            return response;
        }
    }
}
