namespace Reserva.Dto.Dbo.Proveedor
{
    public class UpdateProveedorDto : ProveedorDto
    {
        public int IdProveedor { get; set; }
        public int IdEstadoProveedor { get; set; }
        public string? Telefono { get; set; }
        public string? Facebook { get; set; }
        public string? Instagram { get; set; }
        // Vive en AspNetUsers, no en la entidad Proveedor
        public string? Imagen { get; set; }
    }
}
