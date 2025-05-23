namespace Reserva.Dto.Cancha.Departamento
{
    public class SelectDepartamentoFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdDepartamento { get; set; }
        public bool? Activo { get; set; }
    }
}
