using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Comision;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Comision
{
    public class ListComisionQueryHandler : QueryHandlerBase<ListComisionQuery, IEnumerable<ListComisionDto>>
    {
        private readonly IRepository<Entity.Comision> _repository;

        public ListComisionQueryHandler(
            IMapper mapper,
            IRepository<Entity.Comision> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<ListComisionDto>>> HandleQuery(ListComisionQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListComisionDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.IdComision == request.Id);
            var listDtos = _mapper?.Map<IEnumerable<ListComisionDto>>(list);

            response.UpdateData(listDtos ?? new List<ListComisionDto>());

            return await Task.FromResult(response);
        }
    }
}
