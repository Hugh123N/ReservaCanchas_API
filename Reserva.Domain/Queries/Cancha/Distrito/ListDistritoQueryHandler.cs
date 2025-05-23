using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Distrito;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Distrito
{
    public class ListDistritoQueryHandler : QueryHandlerBase<ListDistritoQuery, IEnumerable<ListDistritoDto>>
    {
        private readonly IRepository<Entity.Models.Distrito> _repository;

        public ListDistritoQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.Distrito> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<ListDistritoDto>>> HandleQuery(ListDistritoQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListDistritoDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.IdDistrito == request.Id);
            var listDtos = _mapper?.Map<IEnumerable<ListDistritoDto>>(list);

            response.UpdateData(listDtos ?? new List<ListDistritoDto>());

            return await Task.FromResult(response);
        }
    }
}
