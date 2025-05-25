using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.EstadoReserva;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.EstadoReserva
{
    public class ListEstadoReservaQueryHandler : QueryHandlerBase<ListEstadoReservaQuery, IEnumerable<ListEstadoReservaDto>>
    {
        private readonly IRepository<Entity.Models.EstadoReserva> _repository;

        public ListEstadoReservaQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.EstadoReserva> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<ListEstadoReservaDto>>> HandleQuery(ListEstadoReservaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListEstadoReservaDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.IdEstadoReserva == request.Id);
            var listDtos = _mapper?.Map<IEnumerable<ListEstadoReservaDto>>(list);

            response.UpdateData(listDtos ?? new List<ListEstadoReservaDto>());

            return await Task.FromResult(response);
        }
    }
}
