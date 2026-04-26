using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.PagoPlan;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.PagoPlan
{
    public class ListPagoPlanQueryHandler : QueryHandlerBase<ListPagoPlanQuery, IEnumerable<ListPagoPlanDto>>
    {
        private readonly IRepository<Entity.PagoPlan> _repository;

        public ListPagoPlanQueryHandler(
            IMapper mapper,
            IRepository<Entity.PagoPlan> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<ListPagoPlanDto>>> HandleQuery(ListPagoPlanQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListPagoPlanDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.IdPagoPlan == request.Id);
            var listDtos = _mapper?.Map<IEnumerable<ListPagoPlanDto>>(list);

            response.UpdateData(listDtos ?? new List<ListPagoPlanDto>());

            return await Task.FromResult(response);
        }
    }
}
