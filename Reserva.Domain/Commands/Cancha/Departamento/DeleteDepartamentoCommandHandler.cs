using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Departamento
{
    public class DeleteDepartamentoCommandHandler : CommandHandlerBase<DeleteDepartamentoCommand>
    {
        private readonly IRepository<Entity.Models.Departamento> _DepartamentoRepository;

        public DeleteDepartamentoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteDepartamentoCommandValidator validator,
            IRepository<Entity.Models.Departamento> DepartamentoRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _DepartamentoRepository = DepartamentoRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteDepartamentoCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var Departamento = await _DepartamentoRepository.GetByAsync(x => x.IdDepartamento == request.Id);

            if (Departamento != null)
            {
                Departamento.Activo = false;
                await _DepartamentoRepository.UpdateAsync(Departamento);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
