namespace Reserva.Dto.Dbo.Calendario
{
    public class DisponibilidadSemanalResponseDto
    {
        public CanchaDisponibilidadDto Cancha { get; set; } = new();
        public List<DiaHorarioDto> Horarios { get; set; } = new();
    }

    public class CanchaDisponibilidadDto
    {
        public int IdCancha { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string HoraInicio { get; set; } = string.Empty;
        public string HoraFin { get; set; } = string.Empty;
    }

    public class DiaHorarioDto
    {
        public string Fecha { get; set; } = string.Empty;
        public int DiaSemana { get; set; }
        public string NombreDia { get; set; } = string.Empty;
        public List<SlotHorarioDto> Slots { get; set; } = new();
    }

    public class SlotHorarioDto
    {
        public int IdHorarioCancha { get; set; }
        public string Hora { get; set; } = string.Empty;
        public string HoraFin { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public ReservaSlotDto? Reserva { get; set; }
    }
    
    public class ReservaSlotDto
    {
        public int IdReserva { get; set; }
        public string CodigoReserva { get; set; } = string.Empty;
        public ClienteSlotDto Cliente { get; set; } = new();
    }

    public class ClienteSlotDto
    {
        public string IdCliente { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
