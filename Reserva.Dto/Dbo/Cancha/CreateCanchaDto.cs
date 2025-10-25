using Reserva.Dto.Dbo.Disponibilidad;
using Reserva.Dto.Dbo.ImagenCancha;
using Reserva.Dto.Dbo.TipoCancha;
using Reserva.Dto.Dbo.Ubigeo;

namespace Reserva.Dto.Dbo.Cancha
{
    public class CreateCanchaDto : CanchaDto
    {
        public List<CreateImagenCanchaDto>? Imagenes { get; set; }
        public List<CreateDisponibilidadDto> Disponibilidades { get; set; } = new List<CreateDisponibilidadDto>();

    }
}
