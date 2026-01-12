using AutoMapper;
using MediatR;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Services;
using Reserva.Dto.Dbo.Cancha;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;
using Microsoft.Data.SqlClient;

namespace Reserva.Domain.Commands.Dbo.Cancha
{
    public class CreateCanchaCommandHandler : CommandHandlerBase<CreateCanchaCommand, GetCanchaDto>
    {
        private readonly IRepository<Entity.Cancha> _CanchaRepository;
        private readonly IRepository<Entity.EstadoCancha> _EstadoCanchaRepository;

        public CreateCanchaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateCanchaCommandValidator validator,
            IRepository<Entity.Cancha> CanchaRepository,
            IRepository<Entity.EstadoCancha> EstadoCanchaRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _CanchaRepository = CanchaRepository;
            _EstadoCanchaRepository = EstadoCanchaRepository;
        }

        public override async Task<ResponseDto<GetCanchaDto>> HandleCommand(CreateCanchaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetCanchaDto>();
            var estadoCancha = await _EstadoCanchaRepository.GetByAsNoTrackingAsync(x => x.Codigo!.Equals(Constants.ESTADO_CANCHA.Pendiente));

            // Expandir horarios: 1 hora → 2 bloques de 30 minutos
            if (request.CreateDto.HorarioCanchas != null && request.CreateDto.HorarioCanchas.Any())
            {
                var horarioCanchaService = new HorarioCanchaService();
                request.CreateDto.HorarioCanchas = horarioCanchaService.ExpandirHorariosCreate(
                    request.CreateDto.HorarioCanchas
                );
            }

            var Cancha = _mapper?.Map<Entity.Cancha>(request.CreateDto);
            Cancha!.IdEstadoCancha = estadoCancha!.IdEstadoCancha;

            if (request.CreateDto.IdsTipoDeportes != null && request.CreateDto.IdsTipoDeportes.Any())
            {
                foreach (var idTipoDeporte in request.CreateDto.IdsTipoDeportes)
                {
                    Cancha.TipoDeporteCancha.Add(new Entity.TipoDeporteCancha
                    {
                        IdTipoDeporte = idTipoDeporte
                    });
                }
            }

            if (request.CreateDto.IdsServicios != null && request.CreateDto.IdsServicios.Any())
            {
                foreach (var idServicio in request.CreateDto.IdsServicios)
                {
                    Cancha.ServicioCancha.Add(new Entity.ServicioCancha
                    {
                        IdServicio = idServicio,
                        EsIncluido = true,
                        CostoAdicional = null
                    });
                }
            }

            Cancha.Codigo = await GenerarCodigoCancha(Cancha.IdProveedor);

            await _CanchaRepository.AddAsync(Cancha);
            await _CanchaRepository.SaveAsync();

            var CanchaDto = _mapper?.Map<GetCanchaDto>(Cancha);
            if (CanchaDto != null) response.UpdateData(CanchaDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }

        private async Task<string> GenerarCodigoCancha(int idProveedor)
        {
            var param = new SqlParameter("@idProveedor", idProveedor);
            var codigo = await _CanchaRepository.ExecuteScalarSPAsync<string>("sp_GenerarCodigoCancha", param);
            if (!string.IsNullOrWhiteSpace(codigo))
                return codigo;
            return string.Empty;
        }
    }
}
