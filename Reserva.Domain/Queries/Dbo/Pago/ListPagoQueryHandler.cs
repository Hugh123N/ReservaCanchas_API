using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Pago;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.Pago
{
    public class ListPagoQueryHandler : QueryHandlerBase<ListPagoQuery, IEnumerable<ListPagoDto>>
    {
        private readonly IRepository<Entity.Pago> _repository;

        public ListPagoQueryHandler(
            IMapper mapper,
            IRepository<Entity.Pago> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<ListPagoDto>>> HandleQuery(ListPagoQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListPagoDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.IdPago == request.Id);
            var listDtos = _mapper?.Map<IEnumerable<ListPagoDto>>(list);

            response.UpdateData(listDtos ?? new List<ListPagoDto>());

            return await Task.FromResult(response);
        }
    }
}
