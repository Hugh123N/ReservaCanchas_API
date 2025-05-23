namespace Reserva.Dto.Cancha.Departamento
{
    public class SearchDepartamentoFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdDepartamento { get; set; }
        public bool? Activo { get; set; }
    }
}
