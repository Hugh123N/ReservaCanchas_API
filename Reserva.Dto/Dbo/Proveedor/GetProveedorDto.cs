namespace Reserva.Dto.Dbo.Proveedor
{
    public class GetProveedorDto : ProveedorDto
    {
        public Guid IdProveedor { get; set; }
        public bool Activo { get; set; }
    }
}
