using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Cancha;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Cancha
{
    public class ListCanchaQueryHandler : QueryHandlerBase<ListCanchaQuery, IEnumerable<ListCanchaDto>>
    {
        private readonly IRepository<Entity.Models.Cancha> _repository;

        public ListCanchaQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.Cancha> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<ListCanchaDto>>> HandleQuery(ListCanchaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListCanchaDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.IdCancha == request.Id);
            var listDtos = _mapper?.Map<IEnumerable<ListCanchaDto>>(list);

            response.UpdateData(listDtos ?? new List<ListCanchaDto>());

            return await Task.FromResult(response);
        }
    }
}
