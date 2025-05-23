using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.TipoProveedor;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.TipoProveedor
{
    public class GetTipoProveedorQueryHandler : QueryHandlerBase<GetTipoProveedorQuery, GetTipoProveedorDto>
    {
        private readonly IRepository<Entity.Models.TipoProveedor> _TipoProveedorRepository;

        public GetTipoProveedorQueryHandler(
            IMapper mapper,
            GetTipoProveedorQueryValidator validator,
            IRepository<Entity.Models.TipoProveedor> TipoProveedorRepository
        ) : base(mapper, validator)
        {
            _TipoProveedorRepository = TipoProveedorRepository;
        }

        protected override async Task<ResponseDto<GetTipoProveedorDto>> HandleQuery(GetTipoProveedorQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetTipoProveedorDto>();
            var TipoProveedor = await _TipoProveedorRepository.GetByAsync(x => x.IdTipoProveedor == request.Id);
            var TipoProveedorDto = _mapper?.Map<GetTipoProveedorDto>(TipoProveedor);

            if (TipoProveedor != null && TipoProveedorDto != null)
            {
                response.UpdateData(TipoProveedorDto);
            }

            return await Task.FromResult(response);
        }
    }
}
