using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Disponibilidad;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Disponibilidad
{
    public class CreateDisponibilidadCommandHandler : CommandHandlerBase<CreateDisponibilidadCommand, GetDisponibilidadDto>
    {
        private readonly IRepository<Entity.Models.Disponibilidad> _DisponibilidadRepository;

        public CreateDisponibilidadCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateDisponibilidadCommandValidator validator,
            IRepository<Entity.Models.Disponibilidad> DisponibilidadRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _DisponibilidadRepository = DisponibilidadRepository;
        }

        public override async Task<ResponseDto<GetDisponibilidadDto>> HandleCommand(CreateDisponibilidadCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetDisponibilidadDto>();

            var Disponibilidad = _mapper?.Map<Entity.Models.Disponibilidad>(request.CreateDto);

            if (Disponibilidad != null)
            {
                await _DisponibilidadRepository.AddAsync(Disponibilidad);
                await _DisponibilidadRepository.SaveAsync();
            }

            var DisponibilidadDto = _mapper?.Map<GetDisponibilidadDto>(Disponibilidad);
            if (DisponibilidadDto != null) response.UpdateData(DisponibilidadDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}