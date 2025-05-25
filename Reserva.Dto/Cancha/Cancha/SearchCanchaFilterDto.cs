namespace Reserva.Dto.Cancha.Cancha
{
    public class SearchCanchaFilterDto
    {
        public string? Nombre { get; set; }
        public string? CodigoDepartamento { get; set; }
        public string? CodigoProvincia { get; set; }
        public string? CodigoDistrito { get; set; }
        public int? IdTipoCancha { get; set; }
        public int? IdEstadoCancha { get; set; }
        public bool? Activo { get; set; }
    }
}
