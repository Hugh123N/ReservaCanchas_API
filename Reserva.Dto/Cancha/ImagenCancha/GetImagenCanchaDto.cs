using Reserva.Dto.Cancha.Cancha;

namespace Reserva.Dto.Cancha.ImagenCancha
{
    public class GetImagenCanchaDto : ImagenCanchaDto
    {
        public int IdImagenCancha { get; set; }
        //public GetCanchaDto? Cancha { get; set; }

        public bool Activo { get; set; }
    }
}
