using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Disponibilidad;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.Disponibilidad
{
    public class ListDisponibilidadQueryHandler : QueryHandlerBase<ListDisponibilidadQuery, IEnumerable<ListDisponibilidadDto>>
    {
        private readonly IRepository<Entity.Disponibilidad> _repository;

        public ListDisponibilidadQueryHandler(
            IMapper mapper,
            IRepository<Entity.Disponibilidad> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<ListDisponibilidadDto>>> HandleQuery(ListDisponibilidadQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListDisponibilidadDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.IdDisponibilidad == request.Id);
            var listDtos = _mapper?.Map<IEnumerable<ListDisponibilidadDto>>(list);

            response.UpdateData(listDtos ?? new List<ListDisponibilidadDto>());

            return await Task.FromResult(response);
        }
    }
}
