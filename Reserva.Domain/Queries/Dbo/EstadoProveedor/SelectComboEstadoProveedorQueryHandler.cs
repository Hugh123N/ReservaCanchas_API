using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.EstadoProveedor;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.EstadoProveedor
{
    public class SelectComboEstadoProveedorQueryHandler : QueryHandlerBase<SelectComboEstadoProveedorQuery, IEnumerable<SelectComboEstadoProveedorDto>>
    {
        private readonly IRepository<Entity.EstadoProveedor> _repository;

        public SelectComboEstadoProveedorQueryHandler(
            IMapper mapper,
            IRepository<Entity.EstadoProveedor> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<SelectComboEstadoProveedorDto>>> HandleQuery(SelectComboEstadoProveedorQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<SelectComboEstadoProveedorDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.Activo);
            var listDtos = _mapper?.Map<IEnumerable<SelectComboEstadoProveedorDto>>(list);

            response.UpdateData(listDtos ?? new List<SelectComboEstadoProveedorDto>());

            return await Task.FromResult(response);
        }
    }
}
