using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.EstadoProveedor;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.EstadoProveedor
{
    public class SelectComboEstadoProveedorQueryHandler : QueryHandlerBase<SelectComboEstadoProveedorQuery, IEnumerable<SelectComboEstadoProveedorDto>>
    {
        private readonly IRepository<Entity.Models.EstadoProveedor> _repository;

        public SelectComboEstadoProveedorQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.EstadoProveedor> repository
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
