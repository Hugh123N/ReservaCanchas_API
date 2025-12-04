using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Servicio;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.Servicio
{
    public class CreateServicioCommandHandler : CommandHandlerBase<CreateServicioCommand, GetServicioDto>
    {
        private readonly IRepository<Entity.Servicio> _ServicioRepository;

        public CreateServicioCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateServicioCommandValidator validator,
            IRepository<Entity.Servicio> ServicioRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _ServicioRepository = ServicioRepository;
        }

        public override async Task<ResponseDto<GetServicioDto>> HandleCommand(CreateServicioCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetServicioDto>();

            var Servicio = _mapper?.Map<Entity.Servicio>(request.CreateDto);

            if (Servicio != null)
            {
                await _ServicioRepository.AddAsync(Servicio);
                await _ServicioRepository.SaveAsync();
            }

            var ServicioDto = _mapper?.Map<GetServicioDto>(Servicio);
            if (ServicioDto != null) response.UpdateData(ServicioDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}