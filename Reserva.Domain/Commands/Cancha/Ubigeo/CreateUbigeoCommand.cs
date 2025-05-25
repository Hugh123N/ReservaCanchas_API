using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Ubigeo;

namespace Reserva.Domain.Commands.Cancha.Ubigeo
{
    public class CreateUbigeoCommand : CommandBase<GetUbigeoDto>
    {
        public CreateUbigeoCommand(CreateUbigeoDto createDto) => CreateDto = createDto;
        public CreateUbigeoDto CreateDto { get; set; }
    }
}
