namespace Reserva.Dto.Dbo.Cancha
{
    public class SearchCanchaFilterDto
    {
        public string? Nombre { get; set; }
        public string? CodigoUbigeo { get; set; }
        public int? IdTipoCancha { get; set; }
        public DateTimeOffset? Fecha { get; set; }
        public string? Hora { get; set; }
        public int? IdEstadoCancha { get; set; }
    }
}
