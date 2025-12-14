namespace Reserva.Dto.Dbo.Operador
{
    public class CreateOperadorDto 
    {
        public int IdProveedor { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Telefono { get; set; }
        public string? Host { get; set; }
        public List<int>? CanchaIds { get; set; }
    }
}
