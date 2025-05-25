using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.EstadoProveedor;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.EstadoProveedor
{
    public class GetEstadoProveedorQueryHandler : QueryHandlerBase<GetEstadoProveedorQuery, GetEstadoProveedorDto>
    {
        private readonly IRepository<Entity.Models.EstadoProveedor> _EstadoProveedorRepository;

        public GetEstadoProveedorQueryHandler(
            IMapper mapper,
            GetEstadoProveedorQueryValidator validator,
            IRepository<Entity.Models.EstadoProveedor> EstadoProveedorRepository
        ) : base(mapper, validator)
        {
            _EstadoProveedorRepository = EstadoProveedorRepository;
        }

        protected override async Task<ResponseDto<GetEstadoProveedorDto>> HandleQuery(GetEstadoProveedorQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetEstadoProveedorDto>();
            var EstadoProveedor = await _EstadoProveedorRepository.GetByAsync(x => x.IdEstadoProveedor == request.Id);
            var EstadoProveedorDto = _mapper?.Map<GetEstadoProveedorDto>(EstadoProveedor);

            if (EstadoProveedor != null && EstadoProveedorDto != null)
            {
                response.UpdateData(EstadoProveedorDto);
            }

            return await Task.FromResult(response);
        }
    }
}
