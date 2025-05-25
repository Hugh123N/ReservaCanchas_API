using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Pago;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Pago
{
    public class ListPagoQueryHandler : QueryHandlerBase<ListPagoQuery, IEnumerable<ListPagoDto>>
    {
        private readonly IRepository<Entity.Models.Pago> _repository;

        public ListPagoQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.Pago> repository
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
