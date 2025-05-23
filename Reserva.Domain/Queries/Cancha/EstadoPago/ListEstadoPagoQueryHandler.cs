using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.EstadoPago;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.EstadoPago
{
    public class ListEstadoPagoQueryHandler : QueryHandlerBase<ListEstadoPagoQuery, IEnumerable<ListEstadoPagoDto>>
    {
        private readonly IRepository<Entity.Models.EstadoPago> _repository;

        public ListEstadoPagoQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.EstadoPago> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<ListEstadoPagoDto>>> HandleQuery(ListEstadoPagoQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListEstadoPagoDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.IdEstadoPago == request.Id);
            var listDtos = _mapper?.Map<IEnumerable<ListEstadoPagoDto>>(list);

            response.UpdateData(listDtos ?? new List<ListEstadoPagoDto>());

            return await Task.FromResult(response);
        }
    }
}
