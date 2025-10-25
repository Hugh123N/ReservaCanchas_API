using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.TipoProveedor;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.TipoProveedor
{
    public class SelectComboTipoProveedorQueryHandler : QueryHandlerBase<SelectComboTipoProveedorQuery, IEnumerable<SelectComboTipoProveedorDto>>
    {
        private readonly IRepository<Entity.TipoProveedor> _repository;

        public SelectComboTipoProveedorQueryHandler(
            IMapper mapper,
            IRepository<Entity.TipoProveedor> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<SelectComboTipoProveedorDto>>> HandleQuery(SelectComboTipoProveedorQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<SelectComboTipoProveedorDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.Activo);
            var listDtos = _mapper?.Map<IEnumerable<SelectComboTipoProveedorDto>>(list);

            response.UpdateData(listDtos ?? new List<SelectComboTipoProveedorDto>());

            return await Task.FromResult(response);
        }
    }
}
