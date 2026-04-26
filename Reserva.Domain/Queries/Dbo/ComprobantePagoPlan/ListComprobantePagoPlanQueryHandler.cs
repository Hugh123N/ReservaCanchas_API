using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.ComprobantePagoPlan;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.ComprobantePagoPlan
{
    public class ListComprobantePagoPlanQueryHandler : QueryHandlerBase<ListComprobantePagoPlanQuery, IEnumerable<ListComprobantePagoPlanDto>>
    {
        private readonly IRepository<Entity.ComprobantePagoPlan> _repository;

        public ListComprobantePagoPlanQueryHandler(
            IMapper mapper,
            IRepository<Entity.ComprobantePagoPlan> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<ListComprobantePagoPlanDto>>> HandleQuery(ListComprobantePagoPlanQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListComprobantePagoPlanDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.IdComprobantePagoPlan == request.Id);
            var listDtos = _mapper?.Map<IEnumerable<ListComprobantePagoPlanDto>>(list);

            response.UpdateData(listDtos ?? new List<ListComprobantePagoPlanDto>());

            return await Task.FromResult(response);
        }
    }
}
