using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.TipoProveedor;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.TipoProveedor
{
    public class ListTipoProveedorQueryHandler : QueryHandlerBase<ListTipoProveedorQuery, IEnumerable<ListTipoProveedorDto>>
    {
        private readonly IRepository<Entity.Models.TipoProveedor> _repository;

        public ListTipoProveedorQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.TipoProveedor> repository
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
