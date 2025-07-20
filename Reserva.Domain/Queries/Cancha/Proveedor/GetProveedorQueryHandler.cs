using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Proveedor;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Proveedor
{
    public class GetProveedorQueryHandler : QueryHandlerBase<GetProveedorQuery, GetProveedorDto>
    {
        private readonly IRepository<Entity.Models.Proveedor> _ProveedorRepository;

        public GetProveedorQueryHandler(
            IMapper mapper,
            GetProveedorQueryValidator validator,
            IRepository<Entity.Models.Proveedor> ProveedorRepository
        ) : base(mapper, validator)
        {
            _ProveedorRepository = ProveedorRepository;
        }

        protected override async Task<ResponseDto<GetProveedorDto>> HandleQuery(GetProveedorQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetProveedorDto>();
            var Proveedor = await _ProveedorRepository.GetByAsync(x => x.IdProveedor == request.Id);
            var ProveedorDto = _mapper?.Map<GetProveedorDto>(Proveedor);

            if (Proveedor != null && ProveedorDto != null)
            {
                response.UpdateData(ProveedorDto);
            }

            return await Task.FromResult(response);
        }
    }
}
