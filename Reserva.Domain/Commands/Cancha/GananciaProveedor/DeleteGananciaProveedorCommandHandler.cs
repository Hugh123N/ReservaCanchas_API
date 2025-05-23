using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.GananciaProveedor
{
    public class DeleteGananciaProveedorCommandHandler : CommandHandlerBase<DeleteGananciaProveedorCommand>
    {
        private readonly IRepository<Entity.Models.GananciaProveedor> _GananciaProveedorRepository;

        public DeleteGananciaProveedorCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteGananciaProveedorCommandValidator validator,
            IRepository<Entity.Models.GananciaProveedor> GananciaProveedorRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _GananciaProveedorRepository = GananciaProveedorRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteGananciaProveedorCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var GananciaProveedor = await _GananciaProveedorRepository.GetByAsync(x => x.IdGananciaProveedor == request.Id);

            if (GananciaProveedor != null)
            {
                GananciaProveedor.Activo = false;
                await _GananciaProveedorRepository.UpdateAsync(GananciaProveedor);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
