namespace Reserva.Dto.Cancha.Proveedor
{
    public class GetProveedorDto : ProveedorDto
    {
        public Guid IdProveedor { get; set; }
        public bool Activo { get; set; }
    }
}
