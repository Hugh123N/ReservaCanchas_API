using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.TipoProveedor;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.TipoProveedor
{
    public class ListTipoProveedorQueryHandler : QueryHandlerBase<ListTipoProveedorQuery, IEnumerable<ListTipoProveedorDto>>
    {
        private readonly IRepository<Entity.TipoProveedor> _repository;

        public ListTipoProveedorQueryHandler(
            IMapper mapper,
            IRepository<Entity.TipoProveedor> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<ListTipoProveedorDto>>> HandleQuery(ListTipoProveedorQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListTipoProveedorDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.IdTipoProveedor == request.Id);
            var listDtos = _mapper?.Map<IEnumerable<ListTipoProveedorDto>>(list);

            response.UpdateData(listDtos ?? new List<ListTipoProveedorDto>());

            return await Task.FromResult(response);
        }
    }
}
