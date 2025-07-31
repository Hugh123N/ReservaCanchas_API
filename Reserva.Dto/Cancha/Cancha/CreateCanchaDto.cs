using Reserva.Dto.Cancha.Disponibilidad;
using Reserva.Dto.Cancha.ImagenCancha;
using Reserva.Dto.Cancha.TipoCancha;
using Reserva.Dto.Cancha.Ubigeo;

namespace Reserva.Dto.Cancha.Cancha
{
    public class CreateCanchaDto : CanchaDto
    {
        //public GetTipoCanchaDto TipoCancha { get; set; } = new GetTipoCanchaDto();
        //public GetUbigeoDto Ubigeo { get; set; } = new GetUbigeoDto();
        public List<CreateImagenCanchaDto>? Imagenes { get; set; }
        public List<CreateDisponibilidadDto> Disponibilidades { get; set; } = new List<CreateDisponibilidadDto>();

    }
}
