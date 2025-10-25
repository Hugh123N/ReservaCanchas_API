using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.EstadoCancha;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.EstadoCancha
{
    public class ListEstadoCanchaQueryHandler : QueryHandlerBase<ListEstadoCanchaQuery, IEnumerable<ListEstadoCanchaDto>>
    {
        private readonly IRepository<Entity.EstadoCancha> _repository;

        public ListEstadoCanchaQueryHandler(
            IMapper mapper,
            IRepository<Entity.EstadoCancha> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<ListEstadoCanchaDto>>> HandleQuery(ListEstadoCanchaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListEstadoCanchaDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.IdEstadoCancha == request.Id);
            var listDtos = _mapper?.Map<IEnumerable<ListEstadoCanchaDto>>(list);

            response.UpdateData(listDtos ?? new List<ListEstadoCanchaDto>());

            return await Task.FromResult(response);
        }
    }
}
