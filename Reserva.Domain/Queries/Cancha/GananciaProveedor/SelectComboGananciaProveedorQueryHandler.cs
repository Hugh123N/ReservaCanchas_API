using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.GananciaProveedor;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.GananciaProveedor
{
    public class SelectComboGananciaProveedorQueryHandler : QueryHandlerBase<SelectComboGananciaProveedorQuery, IEnumerable<SelectComboGananciaProveedorDto>>
    {
        private readonly IRepository<Entity.Models.GananciaProveedor> _repository;

        public SelectComboGananciaProveedorQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.GananciaProveedor> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<SelectComboGananciaProveedorDto>>> HandleQuery(SelectComboGananciaProveedorQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<SelectComboGananciaProveedorDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.Activo);
            var listDtos = _mapper?.Map<IEnumerable<SelectComboGananciaProveedorDto>>(list);

            response.UpdateData(listDtos ?? new List<SelectComboGananciaProveedorDto>());

            return await Task.FromResult(response);
        }
    }
}
