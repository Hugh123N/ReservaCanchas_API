using AutoMapper;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Cancha;
using Reserva.Dto.Dbo.TipoDeporte;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.Cancha
{
    public class GetCanchaConfigQueryHandler : QueryHandlerBase<GetCanchaConfigQuery, GetCanchaConfigDto>
    {
        private readonly IRepository<Entity.Cancha> _canchaRepository;
        private readonly IRepository<Entity.TipoDeporte> _tipoDeporteRepository;

        public GetCanchaConfigQueryHandler(
            IMapper mapper,
            IRepository<Entity.Cancha> canchaRepository,
            IRepository<Entity.TipoDeporte> tipoDeporteRepository
        ) : base(mapper)
        {
            _canchaRepository = canchaRepository;
            _tipoDeporteRepository = tipoDeporteRepository;
        }

        protected override async Task<ResponseDto<GetCanchaConfigDto>> HandleQuery(
            GetCanchaConfigQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetCanchaConfigDto>();

            var cancha = await _canchaRepository.GetByAsync(
                x => x.IdCancha == request.Id && x.Activo,
                x => x.TipoDeporteCancha.Where(td => td.Activo),
                x => x.IdProveedorNavigation.ConfiguracionProveedor);

            if (cancha == null)
            {
                response.AddErrorResult("La cancha no existe o no está activa.");
                return response;
            }

            var configDto = new GetCanchaConfigDto();

            var idsTipoDeporte = cancha.TipoDeporteCancha.Select(td => td.IdTipoDeporte).ToList();
            if (idsTipoDeporte.Any())
            {
                var tipoDeportes = await _tipoDeporteRepository.FindByAsync(
                    td => idsTipoDeporte.Contains(td.IdTipoDeporte));
                configDto.TipoDeportes = _mapper?.Map<List<GetTipoDeporteDto>>(tipoDeportes);
            }

            configDto.PorcentajeAdelantoMinimo = cancha.IdProveedorNavigation.ConfiguracionProveedor?.PorcentajeAdelantoMinimo;

            response.UpdateData(configDto);
            return response;
        }
    }
}
