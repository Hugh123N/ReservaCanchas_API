using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.EstadoProveedor;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.EstadoProveedor
{
    public class ListEstadoProveedorQueryHandler : QueryHandlerBase<ListEstadoProveedorQuery, IEnumerable<ListEstadoProveedorDto>>
    {
        private readonly IRepository<Entity.EstadoProveedor> _repository;

        public ListEstadoProveedorQueryHandler(
            IMapper mapper,
            IRepository<Entity.EstadoProveedor> repository
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
