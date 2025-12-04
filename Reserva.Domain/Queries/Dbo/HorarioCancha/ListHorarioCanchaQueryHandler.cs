using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.HorarioCancha;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.HorarioCancha
{
    public class ListHorarioCanchaQueryHandler : QueryHandlerBase<ListHorarioCanchaQuery, IEnumerable<ListHorarioCanchaDto>>
    {
        private readonly IRepository<Entity.HorarioCancha> _repository;

        public ListHorarioCanchaQueryHandler(
            IMapper mapper,
            IRepository<Entity.HorarioCancha> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<ListHorarioCanchaDto>>> HandleQuery(ListHorarioCanchaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListHorarioCanchaDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.IdHorarioCancha == request.Id);
            var listDtos = _mapper?.Map<IEnumerable<ListHorarioCanchaDto>>(list);

            response.UpdateData(listDtos ?? new List<ListHorarioCanchaDto>());

            return await Task.FromResult(response);
        }
    }
}
