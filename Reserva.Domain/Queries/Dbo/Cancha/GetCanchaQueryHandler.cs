using AutoMapper;
using MediatR;
using Reserva.Domain.Queries.Base;
using Reserva.Domain.Queries.Dbo.HorarioCancha;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Cancha;
using Reserva.Dto.Dbo.HorarioCancha;
using Reserva.Dto.Dbo.Servicio;
using Reserva.Dto.Dbo.TipoDeporte;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Queries.Dbo.Cancha
{
    public class GetCanchaQueryHandler : QueryHandlerBase<GetCanchaQuery, GetCanchaDto>
    {
        private readonly IRepository<Entity.Cancha> _CanchaRepository;
        private readonly IRepository<Entity.Servicio> _ServicioRepository;
        private readonly IRepository<Entity.TipoDeporte> _TipoDeporteRepository;
        private readonly IRepository<Entity.HorarioCancha> _HorarioCanchaRepository;
        private readonly IRepository<Entity.Proveedor> _ProveedorRepository;

        public GetCanchaQueryHandler(
            IMapper mapper,
            IMediator mediator,
            GetCanchaQueryValidator validator,
            IRepository<Entity.Cancha> CanchaRepository,
            IRepository<Entity.Servicio> ServicioRepository,
            IRepository<Entity.TipoDeporte> TipoDeporteRepository,
            IRepository<Entity.HorarioCancha> HorarioCanchaRepository,
            IRepository<Entity.Proveedor> ProveedorRepository
        ) : base(mapper, mediator, validator)
        {
            _CanchaRepository = CanchaRepository;
            _ServicioRepository = ServicioRepository;
            _TipoDeporteRepository = TipoDeporteRepository;
            _HorarioCanchaRepository = HorarioCanchaRepository;
            _ProveedorRepository = ProveedorRepository;
        }

        protected override async Task<ResponseDto<GetCanchaDto>> HandleQuery(GetCanchaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetCanchaDto>();
            var Cancha = await _CanchaRepository.GetByAsync(x => x.IdCancha == request.Id,
                x => x.ImagenCancha.Where(i => i.Activo),
                x => x.IdTipoSuperficieNavigation,
                x => x.ServicioCancha.Where(sc => sc.Activo),
                x => x.TipoDeporteCancha.Where(td => td.Activo),
                x => x.IdEstadoCanchaNavigation,
                x => x.CanchaFavorita.Where(x => x.Activo),
                x => x.CodigoUbigeoNavigation!
            );
            var CanchaDto = _mapper?.Map<GetCanchaDto>(Cancha);

            var idsServicios = Cancha.ServicioCancha.Select(sc => sc.IdServicio).ToList();
            if (idsServicios.Any())
            {
                var servicios = await _ServicioRepository.FindByAsync(
                    s => idsServicios.Contains(s.IdServicio)
                );
                CanchaDto.Servicios = _mapper?.Map<List<GetServicioDto>>(servicios);
            }

            var idsTipoDeporte = Cancha.TipoDeporteCancha.Select(td => td.IdTipoDeporte).ToList();
            if (idsTipoDeporte.Any())
            {
                var tipoDeportes = await _TipoDeporteRepository.FindByAsync(
                    td => idsTipoDeporte.Contains(td.IdTipoDeporte)
                );
                CanchaDto.TipoDeportes = _mapper?.Map<List<GetTipoDeporteDto>>(tipoDeportes);
            }

            var horariosCancha = await _HorarioCanchaRepository.FindByAsync(hc => hc.IdCancha == Cancha.IdCancha,
                x => x.IdDiaSemanaNavigation,
                x => x.IdHoraInicioNavigation
            );
            CanchaDto.HorariosCancha = _mapper?.Map<List<GetHorarioCanchaDto>>(horariosCancha);

            var proveedor = await _ProveedorRepository.GetByAsync(p => p.IdProveedor == Cancha.IdProveedor,
                x => x.ConfiguracionProveedor);

            CanchaDto.DuracionPreReserva = proveedor.ConfiguracionProveedor?.DuracionPreReserva;
            CanchaDto.PorcentajeAdelantoMinimo = proveedor.ConfiguracionProveedor?.PorcentajeAdelantoMinimo;

            response.UpdateData(CanchaDto);

            return await Task.FromResult(response);
        }
    }
}
