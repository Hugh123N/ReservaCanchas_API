using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.DiaSemana;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.DiaSemana
{
    public class ListDiaSemanaQueryHandler : QueryHandlerBase<ListDiaSemanaQuery, IEnumerable<ListDiaSemanaDto>>
    {
        private readonly IRepository<Entity.Models.DiaSemana> _repository;

        public ListDiaSemanaQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.DiaSemana> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<ListDiaSemanaDto>>> HandleQuery(ListDiaSemanaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListDiaSemanaDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.IdDiaSemana == request.Id);
            var listDtos = _mapper?.Map<IEnumerable<ListDiaSemanaDto>>(list);

            response.UpdateData(listDtos ?? new List<ListDiaSemanaDto>());

            return await Task.FromResult(response);
        }
    }
}
