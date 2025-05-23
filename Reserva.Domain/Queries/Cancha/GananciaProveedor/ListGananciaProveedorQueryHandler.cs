using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.GananciaProveedor;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.GananciaProveedor
{
    public class ListGananciaProveedorQueryHandler : QueryHandlerBase<ListGananciaProveedorQuery, IEnumerable<ListGananciaProveedorDto>>
    {
        private readonly IRepository<Entity.Models.GananciaProveedor> _repository;

        public ListGananciaProveedorQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.GananciaProveedor> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<ListGananciaProveedorDto>>> HandleQuery(ListGananciaProveedorQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListGananciaProveedorDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.IdGananciaProveedor == request.Id);
            var listDtos = _mapper?.Map<IEnumerable<ListGananciaProveedorDto>>(list);

            response.UpdateData(listDtos ?? new List<ListGananciaProveedorDto>());

            return await Task.FromResult(response);
        }
    }
}
