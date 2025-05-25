namespace Reserva.Dto.Cancha.TipoProveedor
{
    public class GetTipoProveedorDto : TipoProveedorDto
    {
        public int IdTipoProveedor { get; set; }
        public bool Activo { get; set; }
    }
}
