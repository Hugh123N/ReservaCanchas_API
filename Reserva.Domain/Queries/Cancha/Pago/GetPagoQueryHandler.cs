using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Pago;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Pago
{
    public class GetPagoQueryHandler : QueryHandlerBase<GetPagoQuery, GetPagoDto>
    {
        private readonly IRepository<Entity.Models.Pago> _PagoRepository;

        public GetPagoQueryHandler(
            IMapper mapper,
            GetPagoQueryValidator validator,
            IRepository<Entity.Models.Pago> PagoRepository
        ) : base(mapper, validator)
        {
            _PagoRepository = PagoRepository;
        }

        protected override async Task<ResponseDto<GetPagoDto>> HandleQuery(GetPagoQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetPagoDto>();
            var Pago = await _PagoRepository.GetByAsync(x => x.IdPago == request.Id);
            var PagoDto = _mapper?.Map<GetPagoDto>(Pago);

            if (Pago != null && PagoDto != null)
            {
                response.UpdateData(PagoDto);
            }

            return await Task.FromResult(response);
        }
    }
}
