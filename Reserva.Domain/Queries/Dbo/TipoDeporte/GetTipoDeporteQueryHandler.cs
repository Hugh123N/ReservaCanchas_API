using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.TipoDeporte;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.TipoDeporte
{
    public class GetTipoDeporteQueryHandler : QueryHandlerBase<GetTipoDeporteQuery, GetTipoDeporteDto>
    {
        private readonly IRepository<Entity.TipoDeporte> _TipoDeporteRepository;

        public GetTipoDeporteQueryHandler(
            IMapper mapper,
            GetTipoDeporteQueryValidator validator,
            IRepository<Entity.TipoDeporte> TipoDeporteRepository
        ) : base(mapper, validator)
        {
            _TipoDeporteRepository = TipoDeporteRepository;
        }

        protected override async Task<ResponseDto<GetTipoDeporteDto>> HandleQuery(GetTipoDeporteQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetTipoDeporteDto>();
            var TipoDeporte = await _TipoDeporteRepository.GetByAsync(x => x.IdTipoDeporte == request.Id);
            var TipoDeporteDto = _mapper?.Map<GetTipoDeporteDto>(TipoDeporte);

            if (TipoDeporte != null && TipoDeporteDto != null)
            {
                response.UpdateData(TipoDeporteDto);
            }

            return await Task.FromResult(response);
        }
    }
}
