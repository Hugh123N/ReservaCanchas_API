using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Proveedor;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Proveedor
{
    public class SelectComboProveedorQueryHandler : QueryHandlerBase<SelectComboProveedorQuery, IEnumerable<SelectComboProveedorDto>>
    {
        private readonly IRepository<Entity.Models.Proveedor> _repository;

        public SelectComboProveedorQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.Proveedor> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<SelectComboProveedorDto>>> HandleQuery(SelectComboProveedorQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<SelectComboProveedorDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.Activo);
            var listDtos = _mapper?.Map<IEnumerable<SelectComboProveedorDto>>(list);

            response.UpdateData(listDtos ?? new List<SelectComboProveedorDto>());

            return await Task.FromResult(response);
        }
    }
}
