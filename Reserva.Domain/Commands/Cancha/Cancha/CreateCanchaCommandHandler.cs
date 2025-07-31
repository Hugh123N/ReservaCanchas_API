using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Cancha;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Cancha
{
    public class CreateCanchaCommandHandler : CommandHandlerBase<CreateCanchaCommand, GetCanchaDto>
    {
        private readonly IRepository<Entity.Models.Cancha> _CanchaRepository;
        private readonly IRepository<Entity.Models.Disponibilidad> _DisponibilidadRepository;
        private readonly IRepository<Entity.Models.EstadoCancha> _EstadoCanchaRepository;

        public CreateCanchaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateCanchaCommandValidator validator,
            IRepository<Entity.Models.Cancha> CanchaRepository,
            IRepository<Entity.Models.Disponibilidad> DisponibilidadRepository,
            IRepository<Entity.Models.EstadoCancha> EstadoCanchaRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _CanchaRepository = CanchaRepository;
            _DisponibilidadRepository = DisponibilidadRepository;
            _EstadoCanchaRepository = EstadoCanchaRepository;
        }

        public override async Task<ResponseDto<GetCanchaDto>> HandleCommand(CreateCanchaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetCanchaDto>();
            var estadoCancha = await _EstadoCanchaRepository.GetAsync(Constants.ESTADO_CANCHA.Pendiente);

            var Cancha = _mapper?.Map<Entity.Models.Cancha>(request.CreateDto);
            
            Cancha!.IdEstadoCancha = estadoCancha!.IdEstadoCancha;

            if (Cancha != null)
            {
                await _CanchaRepository.AddAsync(Cancha);
                await _CanchaRepository.SaveAsync();
            }

            var CanchaDto = _mapper?.Map<GetCanchaDto>(Cancha);
            if (CanchaDto != null) response.UpdateData(CanchaDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}