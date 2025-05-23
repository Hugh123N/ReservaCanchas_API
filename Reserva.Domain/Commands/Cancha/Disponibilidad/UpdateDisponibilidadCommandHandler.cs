using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Disponibilidad;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Disponibilidad
{
    public class UpdateDisponibilidadCommandHandler : CommandHandlerBase<UpdateDisponibilidadCommand, GetDisponibilidadDto>
    {
        private readonly IRepository<Entity.Models.Disponibilidad> _DisponibilidadRepository;

        public UpdateDisponibilidadCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateDisponibilidadCommandValidator validator,
            IRepository<Entity.Models.Disponibilidad> DisponibilidadRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _DisponibilidadRepository = DisponibilidadRepository;
        }

        public override async Task<ResponseDto<GetDisponibilidadDto>> HandleCommand(UpdateDisponibilidadCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetDisponibilidadDto>();
            var Disponibilidad = await _DisponibilidadRepository.GetByAsync(x => x.IdDisponibilidad == request.UpdateDto.IdDisponibilidad);

            if (Disponibilidad != null)
            {
                _mapper?.Map(request.UpdateDto, Disponibilidad);
                await _DisponibilidadRepository.UpdateAsync(Disponibilidad);
                await _DisponibilidadRepository.SaveAsync();
            }

            var DisponibilidadDto = _mapper?.Map<GetDisponibilidadDto>(Disponibilidad);
            if (DisponibilidadDto != null) response.UpdateData(DisponibilidadDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
