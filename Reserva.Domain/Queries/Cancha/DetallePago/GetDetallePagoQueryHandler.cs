using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.DetallePago;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.DetallePago
{
    public class GetDetallePagoQueryHandler : QueryHandlerBase<GetDetallePagoQuery, GetDetallePagoDto>
    {
        private readonly IRepository<Entity.Models.DetallePago> _DetallePagoRepository;

        public GetDetallePagoQueryHandler(
            IMapper mapper,
            GetDetallePagoQueryValidator validator,
            IRepository<Entity.Models.DetallePago> DetallePagoRepository
        ) : base(mapper, validator)
        {
            _DetallePagoRepository = DetallePagoRepository;
        }

        protected override async Task<ResponseDto<GetDetallePagoDto>> HandleQuery(GetDetallePagoQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetDetallePagoDto>();
            var DetallePago = await _DetallePagoRepository.GetByAsync(x => x.IdDetallePago == request.Id);
            var DetallePagoDto = _mapper?.Map<GetDetallePagoDto>(DetallePago);

            if (DetallePago != null && DetallePagoDto != null)
            {
                response.UpdateData(DetallePagoDto);
            }

            return await Task.FromResult(response);
        }
    }
}
