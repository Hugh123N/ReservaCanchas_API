using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Cancha;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

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

            var Cancha = _mapper?.Map<Entity.Cancha>(request.CreateDto);
            
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