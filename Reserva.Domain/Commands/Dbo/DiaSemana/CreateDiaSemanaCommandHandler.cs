using AutoMapper;
using MediatR;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.DiaSemana;
using Reserva.Dto.Base;
using Reserva.Entity.Models;
using Reserva.Entity.ITransactions;

namespace Reserva.Domain.Commands.Dbo.DiaSemana
{
    public class CreateDiaSemanaCommandHandler : CommandHandlerBase<CreateDiaSemanaCommand, GetDiaSemanaDto>
    {
        private readonly ReservaCanchasContext _context;

        public CreateDiaSemanaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateDiaSemanaCommandValidator validator,
            ReservaCanchasContext context
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _context = context;
        }

        public override async Task<ResponseDto<GetDiaSemanaDto>> HandleCommand(CreateDiaSemanaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetDiaSemanaDto>();

            var DiaSemana = _mapper?.Map<Entity.Models.DiaSemana>(request.CreateDto);

            if (DiaSemana != null)
            {
                await _context.AddAsync(DiaSemana);
                await _context.SaveChangesAsync();
            }

            var DiaSemanaDto = _mapper?.Map<GetDiaSemanaDto>(DiaSemana);
            if (DiaSemanaDto != null) response.UpdateData(DiaSemanaDto);

            response.AddOkResult("Registro creado exitosamente.");

            return await Task.FromResult(response);
        }
    }
}