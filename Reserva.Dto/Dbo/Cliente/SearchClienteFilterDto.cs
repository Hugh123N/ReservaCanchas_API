namespace Reserva.Dto.Dbo.Cliente
{
    public class SearchClienteFilterDto
    {
        public int? IdProveedor { get; set; }
        public string? Nombre { get; set; } 
        public string? Email { get; set; } 
        public string? Telefono { get; set; }
    }
}
