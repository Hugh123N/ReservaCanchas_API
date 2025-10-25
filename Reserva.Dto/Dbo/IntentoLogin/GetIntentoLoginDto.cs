namespace Reserva.Dto.Dbo.IntentoLogin
{
    public class GetIntentoLoginDto : IntentoLoginDto
    {
        public int IdIntentoLogin { get; set; }
        public bool Activo { get; set; }
    }
}
