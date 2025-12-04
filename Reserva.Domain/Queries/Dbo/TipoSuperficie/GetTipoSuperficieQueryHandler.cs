using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.TipoSuperficie;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.TipoSuperficie
{
    public class GetTipoSuperficieQueryHandler : QueryHandlerBase<GetTipoSuperficieQuery, GetTipoSuperficieDto>
    {
        private readonly IRepository<Entity.TipoSuperficie> _TipoSuperficieRepository;

        public GetTipoSuperficieQueryHandler(
            IMapper mapper,
            GetTipoSuperficieQueryValidator validator,
            IRepository<Entity.TipoSuperficie> TipoSuperficieRepository
        ) : base(mapper, validator)
        {
            _TipoSuperficieRepository = TipoSuperficieRepository;
        }

        protected override async Task<ResponseDto<GetTipoSuperficieDto>> HandleQuery(GetTipoSuperficieQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetTipoSuperficieDto>();
            var TipoSuperficie = await _TipoSuperficieRepository.GetByAsync(x => x.IdTipoSuperficie == request.Id);
            var TipoSuperficieDto = _mapper?.Map<GetTipoSuperficieDto>(TipoSuperficie);

            if (TipoSuperficie != null && TipoSuperficieDto != null)
            {
                response.UpdateData(TipoSuperficieDto);
            }

            return await Task.FromResult(response);
        }
    }
}
