using System;
using System.Collections.Generic;

namespace Reserva.Dto.Cancha.Disponibilidad
{
    public class DisponibilidadDto
    {
        public int Id { get; set; }
        public int IdCancha { get; set; }
        public int IdDiaSemana { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public int? CreadoPor { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
    }
}
