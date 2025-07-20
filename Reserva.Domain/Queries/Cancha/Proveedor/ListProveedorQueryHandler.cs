using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Proveedor;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Proveedor
{
    public class ListProveedorQueryHandler : QueryHandlerBase<ListProveedorQuery, IEnumerable<ListProveedorDto>>
    {
        private readonly IRepository<Entity.Models.Proveedor> _repository;

        public ListProveedorQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.Proveedor> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<ListProveedorDto>>> HandleQuery(ListProveedorQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListProveedorDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.IdProveedor == request.Id);
            var listDtos = _mapper?.Map<IEnumerable<ListProveedorDto>>(list);

            response.UpdateData(listDtos ?? new List<ListProveedorDto>());

            return await Task.FromResult(response);
        }
    }
}
