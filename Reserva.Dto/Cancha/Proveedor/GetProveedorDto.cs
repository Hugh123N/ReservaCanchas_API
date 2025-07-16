namespace Reserva.Dto.Cancha.Proveedor
{
    public class GetProveedorDto : ProveedorDto
    {
        public int IdUsuario { get; set; }
        public bool Activo { get; set; }
    }
}
