using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Ubigeo;

namespace Reserva.Domain.Commands.Dbo.Ubigeo
{
    public class CreateUbigeoCommand : CommandBase<GetUbigeoDto>
    {
        public CreateUbigeoCommand(CreateUbigeoDto createDto) => CreateDto = createDto;
        public CreateUbigeoDto CreateDto { get; set; }
    }
}
