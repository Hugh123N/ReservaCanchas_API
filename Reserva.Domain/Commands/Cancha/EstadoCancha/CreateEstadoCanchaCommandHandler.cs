using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.EstadoCancha;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.EstadoCancha
{
    public class CreateEstadoCanchaCommandHandler : CommandHandlerBase<CreateEstadoCanchaCommand, GetEstadoCanchaDto>
    {
        private readonly IRepository<Entity.Models.EstadoCancha> _EstadoCanchaRepository;

        public CreateEstadoCanchaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateEstadoCanchaCommandValidator validator,
            IRepository<Entity.Models.EstadoCancha> EstadoCanchaRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _EstadoCanchaRepository = EstadoCanchaRepository;
        }

        public override async Task<ResponseDto<GetEstadoCanchaDto>> HandleCommand(CreateEstadoCanchaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetEstadoCanchaDto>();

            var EstadoCancha = _mapper?.Map<Entity.Models.EstadoCancha>(request.CreateDto);

            if (EstadoCancha != null)
            {
                await _EstadoCanchaRepository.AddAsync(EstadoCancha);
                await _EstadoCanchaRepository.SaveAsync();
            }

            var EstadoCanchaDto = _mapper?.Map<GetEstadoCanchaDto>(EstadoCancha);
            if (EstadoCanchaDto != null) response.UpdateData(EstadoCanchaDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}