using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.PagoPlan
{
    public class DeletePagoPlanCommandHandler : CommandHandlerBase<DeletePagoPlanCommand>
    {
        private readonly IRepository<Entity.PagoPlan> _PagoPlanRepository;

        public DeletePagoPlanCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeletePagoPlanCommandValidator validator,
            IRepository<Entity.PagoPlan> PagoPlanRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _PagoPlanRepository = PagoPlanRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeletePagoPlanCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var PagoPlan = await _PagoPlanRepository.GetByAsync(x => x.IdPagoPlan == request.Id);

            if (PagoPlan != null)
            {
                PagoPlan.Activo = false;
                await _PagoPlanRepository.UpdateAsync(PagoPlan);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
