using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.PagoPlan;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.PagoPlan
{
    public class GetPagoPlanQueryHandler : QueryHandlerBase<GetPagoPlanQuery, GetPagoPlanDto>
    {
        private readonly IRepository<Entity.PagoPlan> _PagoPlanRepository;

        public GetPagoPlanQueryHandler(
            IMapper mapper,
            GetPagoPlanQueryValidator validator,
            IRepository<Entity.PagoPlan> PagoPlanRepository
        ) : base(mapper, validator)
        {
            _PagoPlanRepository = PagoPlanRepository;
        }

        protected override async Task<ResponseDto<GetPagoPlanDto>> HandleQuery(GetPagoPlanQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetPagoPlanDto>();
            var PagoPlan = await _PagoPlanRepository.GetByAsync(x => x.IdPagoPlan == request.Id);
            var PagoPlanDto = _mapper?.Map<GetPagoPlanDto>(PagoPlan);

            if (PagoPlan != null && PagoPlanDto != null)
            {
                response.UpdateData(PagoPlanDto);
            }

            return await Task.FromResult(response);
        }
    }
}
