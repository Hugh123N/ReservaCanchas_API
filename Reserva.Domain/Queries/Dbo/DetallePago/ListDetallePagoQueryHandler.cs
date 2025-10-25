using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.DetallePago;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.DetallePago
{
    public class ListDetallePagoQueryHandler : QueryHandlerBase<ListDetallePagoQuery, IEnumerable<ListDetallePagoDto>>
    {
        private readonly IRepository<Entity.DetallePago> _repository;

        public ListDetallePagoQueryHandler(
            IMapper mapper,
            IRepository<Entity.DetallePago> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<ListDetallePagoDto>>> HandleQuery(ListDetallePagoQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListDetallePagoDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.IdDetallePago == request.Id);
            var listDtos = _mapper?.Map<IEnumerable<ListDetallePagoDto>>(list);

            response.UpdateData(listDtos ?? new List<ListDetallePagoDto>());

            return await Task.FromResult(response);
        }
    }
}
