using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.DetallePago;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.DetallePago
{
    public class ListDetallePagoQueryHandler : QueryHandlerBase<ListDetallePagoQuery, IEnumerable<ListDetallePagoDto>>
    {
        private readonly IRepository<Entity.Models.DetallePago> _repository;

        public ListDetallePagoQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.DetallePago> repository
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
