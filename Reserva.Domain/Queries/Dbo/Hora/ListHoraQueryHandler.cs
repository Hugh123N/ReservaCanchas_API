using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Hora;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.Hora
{
    public class ListHoraQueryHandler : QueryHandlerBase<ListHoraQuery, IEnumerable<ListHoraDto>>
    {
        private readonly IRepository<Entity.Hora> _repository;

        public ListHoraQueryHandler(
            IMapper mapper,
            IRepository<Entity.Hora> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<ListHoraDto>>> HandleQuery(ListHoraQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListHoraDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.IdHora == request.Id);
            var listDtos = _mapper?.Map<IEnumerable<ListHoraDto>>(list);

            response.UpdateData(listDtos ?? new List<ListHoraDto>());

            return await Task.FromResult(response);
        }
    }
}
