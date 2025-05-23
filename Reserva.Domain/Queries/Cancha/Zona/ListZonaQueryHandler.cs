using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Zona;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Zona
{
    public class ListZonaQueryHandler : QueryHandlerBase<ListZonaQuery, IEnumerable<ListZonaDto>>
    {
        private readonly IRepository<Entity.Models.Zona> _repository;

        public ListZonaQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.Zona> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<ListZonaDto>>> HandleQuery(ListZonaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListZonaDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.IdZona == request.Id);
            var listDtos = _mapper?.Map<IEnumerable<ListZonaDto>>(list);

            response.UpdateData(listDtos ?? new List<ListZonaDto>());

            return await Task.FromResult(response);
        }
    }
}
