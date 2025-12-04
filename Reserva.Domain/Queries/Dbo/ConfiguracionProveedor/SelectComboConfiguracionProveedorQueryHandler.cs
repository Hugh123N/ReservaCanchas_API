using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.ConfiguracionProveedor;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.ConfiguracionProveedor
{
    public class SelectComboConfiguracionProveedorQueryHandler : QueryHandlerBase<SelectComboConfiguracionProveedorQuery, IEnumerable<SelectComboConfiguracionProveedorDto>>
    {
        private readonly IRepository<Entity.ConfiguracionProveedor> _repository;

        public SelectComboConfiguracionProveedorQueryHandler(
            IMapper mapper,
            IRepository<Entity.ConfiguracionProveedor> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<SelectComboConfiguracionProveedorDto>>> HandleQuery(SelectComboConfiguracionProveedorQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<SelectComboConfiguracionProveedorDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.Activo);
            var listDtos = _mapper?.Map<IEnumerable<SelectComboConfiguracionProveedorDto>>(list);

            response.UpdateData(listDtos ?? new List<SelectComboConfiguracionProveedorDto>());

            return await Task.FromResult(response);
        }
    }
}
