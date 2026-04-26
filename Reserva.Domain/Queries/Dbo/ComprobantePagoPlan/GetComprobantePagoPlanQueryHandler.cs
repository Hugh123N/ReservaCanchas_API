using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.ComprobantePagoPlan;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.ComprobantePagoPlan
{
    public class GetComprobantePagoPlanQueryHandler : QueryHandlerBase<GetComprobantePagoPlanQuery, GetComprobantePagoPlanDto>
    {
        private readonly IRepository<Entity.ComprobantePagoPlan> _ComprobantePagoPlanRepository;

        public GetComprobantePagoPlanQueryHandler(
            IMapper mapper,
            GetComprobantePagoPlanQueryValidator validator,
            IRepository<Entity.ComprobantePagoPlan> ComprobantePagoPlanRepository
        ) : base(mapper, validator)
        {
            _ComprobantePagoPlanRepository = ComprobantePagoPlanRepository;
        }

        protected override async Task<ResponseDto<GetComprobantePagoPlanDto>> HandleQuery(GetComprobantePagoPlanQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetComprobantePagoPlanDto>();
            var ComprobantePagoPlan = await _ComprobantePagoPlanRepository.GetByAsync(x => x.IdComprobantePagoPlan == request.Id);
            var ComprobantePagoPlanDto = _mapper?.Map<GetComprobantePagoPlanDto>(ComprobantePagoPlan);

            if (ComprobantePagoPlan != null && ComprobantePagoPlanDto != null)
            {
                response.UpdateData(ComprobantePagoPlanDto);
            }

            return await Task.FromResult(response);
        }
    }
}
