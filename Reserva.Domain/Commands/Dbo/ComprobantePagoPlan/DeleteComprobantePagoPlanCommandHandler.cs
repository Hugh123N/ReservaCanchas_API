using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.ComprobantePagoPlan
{
    public class DeleteComprobantePagoPlanCommandHandler : CommandHandlerBase<DeleteComprobantePagoPlanCommand>
    {
        private readonly IRepository<Entity.ComprobantePagoPlan> _ComprobantePagoPlanRepository;

        public DeleteComprobantePagoPlanCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteComprobantePagoPlanCommandValidator validator,
            IRepository<Entity.ComprobantePagoPlan> ComprobantePagoPlanRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _ComprobantePagoPlanRepository = ComprobantePagoPlanRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteComprobantePagoPlanCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var ComprobantePagoPlan = await _ComprobantePagoPlanRepository.GetByAsync(x => x.IdComprobantePagoPlan == request.Id);

            if (ComprobantePagoPlan != null)
            {
                ComprobantePagoPlan.Activo = false;
                await _ComprobantePagoPlanRepository.UpdateAsync(ComprobantePagoPlan);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
