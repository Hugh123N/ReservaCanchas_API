namespace Reserva.Dto.Dbo.Calendario
{
    public class ClienteReservaDto
    {
        //ID del cliente (si ya existe). Null si es nuevo cliente
        public Guid? IdCliente { get; set; }
        public string? Nombre { get; set; }
        public string? Apellidos { get; set; }
        public string Telefono { get; set; } = null!;
        public string? Email { get; set; }
        public bool EsNuevoCliente { get; set; }
    }
}
