using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.MetodoPago;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.MetodoPago
{
    public class ListMetodoPagoQueryHandler : QueryHandlerBase<ListMetodoPagoQuery, IEnumerable<ListMetodoPagoDto>>
    {
        private readonly IRepository<Entity.Models.MetodoPago> _repository;

        public ListMetodoPagoQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.MetodoPago> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<ListMetodoPagoDto>>> HandleQuery(ListMetodoPagoQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListMetodoPagoDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.IdMetodoPago == request.Id);
            var listDtos = _mapper?.Map<IEnumerable<ListMetodoPagoDto>>(list);

            response.UpdateData(listDtos ?? new List<ListMetodoPagoDto>());

            return await Task.FromResult(response);
        }
    }
}
