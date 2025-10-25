using Reserva.Dto.Dbo.Cancha;

namespace Reserva.Dto.Dbo.ImagenCancha
{
    public class GetImagenCanchaDto : ImagenCanchaDto
    {
        public int IdImagenCancha { get; set; }
        //public GetCanchaDto? Cancha { get; set; }

        public bool Activo { get; set; }
    }
}
