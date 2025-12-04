using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.DetalleReserva;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.DetalleReserva
{
    public class ListDetalleReservaQueryHandler : QueryHandlerBase<ListDetalleReservaQuery, IEnumerable<ListDetalleReservaDto>>
    {
        private readonly IRepository<Entity.DetalleReserva> _repository;

        public ListDetalleReservaQueryHandler(
            IMapper mapper,
            IRepository<Entity.DetalleReserva> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<ListDetalleReservaDto>>> HandleQuery(ListDetalleReservaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListDetalleReservaDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.IdDetalleReserva == request.Id);
            var listDtos = _mapper?.Map<IEnumerable<ListDetalleReservaDto>>(list);

            response.UpdateData(listDtos ?? new List<ListDetalleReservaDto>());

            return await Task.FromResult(response);
        }
    }
}
