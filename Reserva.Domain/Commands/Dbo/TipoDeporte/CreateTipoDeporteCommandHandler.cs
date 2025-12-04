using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.TipoDeporte;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.TipoDeporte
{
    public class CreateTipoDeporteCommandHandler : CommandHandlerBase<CreateTipoDeporteCommand, GetTipoDeporteDto>
    {
        private readonly IRepository<Entity.TipoDeporte> _TipoDeporteRepository;

        public CreateTipoDeporteCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateTipoDeporteCommandValidator validator,
            IRepository<Entity.TipoDeporte> TipoDeporteRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _TipoDeporteRepository = TipoDeporteRepository;
        }

        public override async Task<ResponseDto<GetTipoDeporteDto>> HandleCommand(CreateTipoDeporteCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetTipoDeporteDto>();

            var TipoDeporte = _mapper?.Map<Entity.TipoDeporte>(request.CreateDto);

            if (TipoDeporte != null)
            {
                await _TipoDeporteRepository.AddAsync(TipoDeporte);
                await _TipoDeporteRepository.SaveAsync();
            }

            var TipoDeporteDto = _mapper?.Map<GetTipoDeporteDto>(TipoDeporte);
            if (TipoDeporteDto != null) response.UpdateData(TipoDeporteDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}