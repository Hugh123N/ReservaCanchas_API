using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.MetodoPago;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.MetodoPago
{
    public class GetMetodoPagoQueryHandler : QueryHandlerBase<GetMetodoPagoQuery, GetMetodoPagoDto>
    {
        private readonly IRepository<Entity.Models.MetodoPago> _MetodoPagoRepository;

        public GetMetodoPagoQueryHandler(
            IMapper mapper,
            GetMetodoPagoQueryValidator validator,
            IRepository<Entity.Models.MetodoPago> MetodoPagoRepository
        ) : base(mapper, validator)
        {
            _MetodoPagoRepository = MetodoPagoRepository;
        }

        protected override async Task<ResponseDto<GetMetodoPagoDto>> HandleQuery(GetMetodoPagoQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetMetodoPagoDto>();
            var MetodoPago = await _MetodoPagoRepository.GetByAsync(x => x.IdMetodoPago == request.Id);
            var MetodoPagoDto = _mapper?.Map<GetMetodoPagoDto>(MetodoPago);

            if (MetodoPago != null && MetodoPagoDto != null)
            {
                response.UpdateData(MetodoPagoDto);
            }

            return await Task.FromResult(response);
        }
    }
}
