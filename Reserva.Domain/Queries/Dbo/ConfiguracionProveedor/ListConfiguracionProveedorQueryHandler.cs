using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.ConfiguracionProveedor;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.ConfiguracionProveedor
{
    public class ListConfiguracionProveedorQueryHandler : QueryHandlerBase<ListConfiguracionProveedorQuery, IEnumerable<ListConfiguracionProveedorDto>>
    {
        private readonly IRepository<Entity.ConfiguracionProveedor> _repository;

        public ListConfiguracionProveedorQueryHandler(
            IMapper mapper,
            IRepository<Entity.ConfiguracionProveedor> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<ListConfiguracionProveedorDto>>> HandleQuery(ListConfiguracionProveedorQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListConfiguracionProveedorDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.IdConfiguracionProveedor == request.Id);
            var listDtos = _mapper?.Map<IEnumerable<ListConfiguracionProveedorDto>>(list);

            response.UpdateData(listDtos ?? new List<ListConfiguracionProveedorDto>());

            return await Task.FromResult(response);
        }
    }
}
