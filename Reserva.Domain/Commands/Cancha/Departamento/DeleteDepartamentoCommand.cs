using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.Departamento
{
    public class DeleteDepartamentoCommand : CommandBase
    {
        public DeleteDepartamentoCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
