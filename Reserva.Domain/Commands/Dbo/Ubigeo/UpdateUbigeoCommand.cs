using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Ubigeo;

namespace Reserva.Domain.Commands.Dbo.Ubigeo
{
    public class UpdateUbigeoCommand : CommandBase<GetUbigeoDto>
    {
        public UpdateUbigeoCommand(UpdateUbigeoDto updateDto) => UpdateDto = updateDto;
        public UpdateUbigeoDto UpdateDto { get; set; }
    }
}
