using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.ConfiguracionProveedor;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.ConfiguracionProveedor
{
    public class GetConfiguracionProveedorQueryHandler : QueryHandlerBase<GetConfiguracionProveedorQuery, GetConfiguracionProveedorDto>
    {
        private readonly IRepository<Entity.ConfiguracionProveedor> _ConfiguracionProveedorRepository;

        public GetConfiguracionProveedorQueryHandler(
            IMapper mapper,
            GetConfiguracionProveedorQueryValidator validator,
            IRepository<Entity.ConfiguracionProveedor> ConfiguracionProveedorRepository
        ) : base(mapper, validator)
        {
            _ConfiguracionProveedorRepository = ConfiguracionProveedorRepository;
        }

        protected override async Task<ResponseDto<GetConfiguracionProveedorDto>> HandleQuery(GetConfiguracionProveedorQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetConfiguracionProveedorDto>();
            var ConfiguracionProveedor = await _ConfiguracionProveedorRepository.GetByAsync(x => x.IdConfiguracionProveedor == request.Id);
            var ConfiguracionProveedorDto = _mapper?.Map<GetConfiguracionProveedorDto>(ConfiguracionProveedor);

            if (ConfiguracionProveedor != null && ConfiguracionProveedorDto != null)
            {
                response.UpdateData(ConfiguracionProveedorDto);
            }

            return await Task.FromResult(response);
        }
    }
}
