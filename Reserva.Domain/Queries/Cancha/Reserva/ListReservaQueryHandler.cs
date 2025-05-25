using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Reserva;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Reserva
{
    public class ListReservaQueryHandler : QueryHandlerBase<ListReservaQuery, IEnumerable<ListReservaDto>>
    {
        private readonly IRepository<Entity.Models.Reserva> _repository;

        public ListReservaQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.Reserva> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<ListReservaDto>>> HandleQuery(ListReservaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListReservaDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.IdReserva == request.Id);
            var listDtos = _mapper?.Map<IEnumerable<ListReservaDto>>(list);

            response.UpdateData(listDtos ?? new List<ListReservaDto>());

            return await Task.FromResult(response);
        }
    }
}
