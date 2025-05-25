using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.EstadoPago;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.EstadoPago
{
    public class GetEstadoPagoQueryHandler : QueryHandlerBase<GetEstadoPagoQuery, GetEstadoPagoDto>
    {
        private readonly IRepository<Entity.Models.EstadoPago> _EstadoPagoRepository;

        public GetEstadoPagoQueryHandler(
            IMapper mapper,
            GetEstadoPagoQueryValidator validator,
            IRepository<Entity.Models.EstadoPago> EstadoPagoRepository
        ) : base(mapper, validator)
        {
            _EstadoPagoRepository = EstadoPagoRepository;
        }

        protected override async Task<ResponseDto<GetEstadoPagoDto>> HandleQuery(GetEstadoPagoQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetEstadoPagoDto>();
            var EstadoPago = await _EstadoPagoRepository.GetByAsync(x => x.IdEstadoPago == request.Id);
            var EstadoPagoDto = _mapper?.Map<GetEstadoPagoDto>(EstadoPago);

            if (EstadoPago != null && EstadoPagoDto != null)
            {
                response.UpdateData(EstadoPagoDto);
            }

            return await Task.FromResult(response);
        }
    }
}
