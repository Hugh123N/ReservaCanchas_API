using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.EstadoProveedor;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.EstadoProveedor
{
    public class ListEstadoProveedorQueryHandler : QueryHandlerBase<ListEstadoProveedorQuery, IEnumerable<ListEstadoProveedorDto>>
    {
        private readonly IRepository<Entity.Models.EstadoProveedor> _repository;

        public ListEstadoProveedorQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.EstadoProveedor> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<ListEstadoProveedorDto>>> HandleQuery(ListEstadoProveedorQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListEstadoProveedorDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.IdEstadoProveedor == request.Id);
            var listDtos = _mapper?.Map<IEnumerable<ListEstadoProveedorDto>>(list);

            response.UpdateData(listDtos ?? new List<ListEstadoProveedorDto>());

            return await Task.FromResult(response);
        }
    }
}
