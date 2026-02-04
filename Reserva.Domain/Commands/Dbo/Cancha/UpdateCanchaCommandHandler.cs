using AutoMapper;
using Reclutamiento.Domain.Extensions;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Services;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Cancha;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.Cancha
{
    public class UpdateCanchaCommandHandler : CommandHandlerBase<UpdateCanchaCommand, GetCanchaDto>
    {
        private readonly IRepository<Entity.Cancha> _CanchaRepository;
        private readonly IRepository<Entity.TipoDeporteCancha> _TipoDeporteCanchaRepository;
        private readonly IRepository<Entity.ServicioCancha> _ServicioCanchaRepository;

        public UpdateCanchaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateCanchaCommandValidator validator,
            IRepository<Entity.Cancha> CanchaRepository,
            IRepository<Entity.TipoDeporteCancha> TipoDeporteCanchaRepository,
            IRepository<Entity.ServicioCancha> ServicioCanchaRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _CanchaRepository = CanchaRepository;
            _TipoDeporteCanchaRepository = TipoDeporteCanchaRepository;
            _ServicioCanchaRepository = ServicioCanchaRepository;
        }

        public override async Task<ResponseDto<GetCanchaDto>> HandleCommand(UpdateCanchaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetCanchaDto>();

            var Cancha = await _CanchaRepository.GetByAsync(x => x.IdCancha == request.UpdateDto.IdCancha,
                x => x.HorarioCancha, 
                x => x.TipoDeporteCancha,
                x => x.ServicioCancha);

            if (request.UpdateDto.HorarioCanchas != null && request.UpdateDto.HorarioCanchas.Any())
            {
                // Expandir horarios: 1 hora → 2 bloques de 30 minutos
                // Pasar horarios existentes para buscar IDs del segundo bloque por combinación única
                var horarioCanchaService = new HorarioCanchaService();
                request.UpdateDto.HorarioCanchas = horarioCanchaService.ExpandirHorariosUpdate(
                    request.UpdateDto.HorarioCanchas,
                    Cancha.HorarioCancha
                );

                Cancha.HorarioCancha.ActualizarColeccion(
                   request.UpdateDto.HorarioCanchas,
                   e => e.IdHorarioCancha,
                   d => d.IdHorarioCancha,
                   (dto, entidad) => {
                       _mapper.Map(dto, entidad);
                       entidad.Activo = true; 
                   },
                   dto => {
                       var nuevo = _mapper.Map<Entity.HorarioCancha>(dto);
                       nuevo.IdCancha = Cancha.IdCancha;
                       nuevo.UserNameCreate = Cancha.UserNameCreate;
                       nuevo.CreateDate = Cancha.CreateDate;
                       nuevo.Activo = true;
                       return nuevo;
                   }
               );
            }

            if (request.UpdateDto.IdsTipoDeportes != null)
            {
                Cancha.TipoDeporteCancha.Where(x => x.Activo && !request.UpdateDto.IdsTipoDeportes.Contains(x.IdTipoDeporte))
                    .ToList().ForEach(x => x.Activo = false);

                foreach (var idTipoDeporte in request.UpdateDto.IdsTipoDeportes)
                {
                    var existente = Cancha.TipoDeporteCancha.FirstOrDefault(x => x.IdTipoDeporte == idTipoDeporte);
                    if (existente != null)
                    {
                        existente.Activo = true;
                    }
                    else
                    {
                        Cancha.TipoDeporteCancha.Add(new Entity.TipoDeporteCancha
                        {
                            IdTipoDeporte = idTipoDeporte,
                            Activo = true
                        });
                    }
                }
            }

            if (request.UpdateDto.IdsServicios != null)
            {
                Cancha.ServicioCancha.Where(x => x.Activo && !request.UpdateDto.IdsServicios.Contains(x.IdServicio))
                    .ToList().ForEach(x => x.Activo = false);

                foreach (var idServicio in request.UpdateDto.IdsServicios)
                {
                    var existente = Cancha.ServicioCancha.FirstOrDefault(x => x.IdServicio == idServicio);
                    if (existente != null)
                    {
                        existente.Activo = true;
                    }
                    else
                    {
                        Cancha.ServicioCancha.Add(new Entity.ServicioCancha
                        {
                            IdServicio = idServicio,
                            EsIncluido = true,
                            CostoAdicional = null,
                            Activo = true
                        });
                    }
                }
            }

            _mapper?.Map(request.UpdateDto, Cancha);

            await _CanchaRepository.UpdateAsync(Cancha);
            await _CanchaRepository.SaveAsync();

            var CanchaDto = _mapper?.Map<GetCanchaDto>(Cancha);
            if (CanchaDto != null) response.UpdateData(CanchaDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);
            return await Task.FromResult(response);
        }
    }
}
