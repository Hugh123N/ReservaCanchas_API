namespace Reserva.Dto.Dbo.Reclamacion
{
    public class SearchReclamacionDto
    {
        public int IdReclamacion { get; set; }
        public string NumeroHoja { get; set; } = "";
        public DateTimeOffset FechaRegistro { get; set; }
        public string Nombres { get; set; } = "";
        public string Tipo { get; set; } = "";
        public string Estado { get; set; } = "";
    }
}
