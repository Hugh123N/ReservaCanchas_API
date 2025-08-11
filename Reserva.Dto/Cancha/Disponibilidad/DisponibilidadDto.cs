using System;
using System.Collections.Generic;

namespace Reserva.Dto.Cancha.Disponibilidad
{
    public class DisponibilidadDto
    {
        public int? IdCancha { get; set; }
        public int IdDiaSemana { get; set; }
        public TimeOnly HoraInicio { get; set; }
        public TimeOnly HoraFin { get; set; }
        public bool? Disponible { get; set; }
    }
}
