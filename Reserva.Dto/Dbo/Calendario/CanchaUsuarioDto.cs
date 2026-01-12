namespace Reserva.Dto.Dbo.Calendario
{
    /// <summary>
    /// DTO para representar una cancha asociada a un usuario (Proveedor u Operador)
    /// </summary>
    public class CanchaUsuarioDto
    {
        public int IdCancha { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int IdProveedor { get; set; }
        public int IdTipoCancha { get; set; }
        public string TipoCancha { get; set; } = string.Empty;
    }
}
