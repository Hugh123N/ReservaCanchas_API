using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.GananciaProveedor;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.GananciaProveedor
{
    public class GetGananciaProveedorQueryHandler : QueryHandlerBase<GetGananciaProveedorQuery, GetGananciaProveedorDto>
    {
        private readonly IRepository<Entity.Models.GananciaProveedor> _GananciaProveedorRepository;

        public GetGananciaProveedorQueryHandler(
            IMapper mapper,
            GetGananciaProveedorQueryValidator validator,
            IRepository<Entity.Models.GananciaProveedor> GananciaProveedorRepository
        ) : base(mapper, validator)
        {
            _GananciaProveedorRepository = GananciaProveedorRepository;
        }

        protected override async Task<ResponseDto<GetGananciaProveedorDto>> HandleQuery(GetGananciaProveedorQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetGananciaProveedorDto>();
            var GananciaProveedor = await _GananciaProveedorRepository.GetByAsync(x => x.IdGananciaProveedor == request.Id);
            var GananciaProveedorDto = _mapper?.Map<GetGananciaProveedorDto>(GananciaProveedor);

            if (GananciaProveedor != null && GananciaProveedorDto != null)
            {
                response.UpdateData(GananciaProveedorDto);
            }

            return await Task.FromResult(response);
        }
    }
}
