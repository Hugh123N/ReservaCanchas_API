using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Zona;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Zona
{
    public class CreateZonaCommandHandler : CommandHandlerBase<CreateZonaCommand, GetZonaDto>
    {
        private readonly IRepository<Entity.Models.Zona> _ZonaRepository;

        public CreateZonaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateZonaCommandValidator validator,
            IRepository<Entity.Models.Zona> ZonaRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _ZonaRepository = ZonaRepository;
        }

        public override async Task<ResponseDto<GetZonaDto>> HandleCommand(CreateZonaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetZonaDto>();

            var Zona = _mapper?.Map<Entity.Models.Zona>(request.CreateDto);

            if (Zona != null)
            {
                await _ZonaRepository.AddAsync(Zona);
                await _ZonaRepository.SaveAsync();
            }

            var ZonaDto = _mapper?.Map<GetZonaDto>(Zona);
            if (ZonaDto != null) response.UpdateData(ZonaDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}