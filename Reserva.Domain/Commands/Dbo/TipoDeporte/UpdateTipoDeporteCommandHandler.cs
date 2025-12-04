using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.TipoDeporte;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.TipoDeporte
{
    public class UpdateTipoDeporteCommandHandler : CommandHandlerBase<UpdateTipoDeporteCommand, GetTipoDeporteDto>
    {
        private readonly IRepository<Entity.TipoDeporte> _TipoDeporteRepository;

        public UpdateTipoDeporteCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateTipoDeporteCommandValidator validator,
            IRepository<Entity.TipoDeporte> TipoDeporteRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _TipoDeporteRepository = TipoDeporteRepository;
        }

        public override async Task<ResponseDto<GetTipoDeporteDto>> HandleCommand(UpdateTipoDeporteCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetTipoDeporteDto>();
            var TipoDeporte = await _TipoDeporteRepository.GetByAsync(x => x.IdTipoDeporte == request.UpdateDto.IdTipoDeporte);

            if (TipoDeporte != null)
            {
                _mapper?.Map(request.UpdateDto, TipoDeporte);
                await _TipoDeporteRepository.UpdateAsync(TipoDeporte);
                await _TipoDeporteRepository.SaveAsync();
            }

            var TipoDeporteDto = _mapper?.Map<GetTipoDeporteDto>(TipoDeporte);
            if (TipoDeporteDto != null) response.UpdateData(TipoDeporteDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
