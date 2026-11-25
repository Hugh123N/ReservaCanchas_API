using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Reclamacion;

namespace Reserva.Domain.Commands.Dbo.Reclamacion
{
    public class AtenderReclamacionCommand : CommandBase
    {
        public AtenderReclamacionCommand(int id, AtenderReclamacionDto atenderDto)
        {
            Id = id;
            AtenderDto = atenderDto;
        }

        public int Id { get; set; }
        public AtenderReclamacionDto AtenderDto { get; set; }
    }
}
