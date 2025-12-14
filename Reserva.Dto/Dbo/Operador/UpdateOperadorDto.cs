namespace Reserva.Dto.Dbo.Operador
{
    public class UpdateOperadorDto
    {
        public int IdOperador { get; set; }
        public int IdProveedor { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Telefono { get; set; }
        public List<int>? CanchaIds { get; set; }
    }
}
