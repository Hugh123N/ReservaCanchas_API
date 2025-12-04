using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.DetalleReserva;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.DetalleReserva
{
    public class GetDetalleReservaQueryHandler : QueryHandlerBase<GetDetalleReservaQuery, GetDetalleReservaDto>
    {
        private readonly IRepository<Entity.DetalleReserva> _DetalleReservaRepository;

        public GetDetalleReservaQueryHandler(
            IMapper mapper,
            GetDetalleReservaQueryValidator validator,
            IRepository<Entity.DetalleReserva> DetalleReservaRepository
        ) : base(mapper, validator)
        {
            _DetalleReservaRepository = DetalleReservaRepository;
        }

        protected override async Task<ResponseDto<GetDetalleReservaDto>> HandleQuery(GetDetalleReservaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetDetalleReservaDto>();
            var DetalleReserva = await _DetalleReservaRepository.GetByAsync(x => x.IdDetalleReserva == request.Id);
            var DetalleReservaDto = _mapper?.Map<GetDetalleReservaDto>(DetalleReserva);

            if (DetalleReserva != null && DetalleReservaDto != null)
            {
                response.UpdateData(DetalleReservaDto);
            }

            return await Task.FromResult(response);
        }
    }
}
