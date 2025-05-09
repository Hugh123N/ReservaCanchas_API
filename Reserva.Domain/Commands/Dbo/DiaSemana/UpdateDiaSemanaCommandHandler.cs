using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.DiaSemana;
using Reserva.Entity.Models;
using Reserva.Entity.ITransactions;
using Microsoft.EntityFrameworkCore;

namespace Reserva.Domain.Commands.Dbo.DiaSemana
{
    public class UpdateDiaSemanaCommandHandler : CommandHandlerBase<UpdateDiaSemanaCommand, GetDiaSemanaDto>
    {
        private readonly ReservaCanchasContext _context;

        public UpdateDiaSemanaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateDiaSemanaCommandValidator validator,
            ReservaCanchasContext context
        ) : base(unitOfWork, mapper, validator)
        {
            _context = context;
        }

        public override async Task<ResponseDto<GetDiaSemanaDto>> HandleCommand(UpdateDiaSemanaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetDiaSemanaDto>();
            var DiaSemana = await _context.DiaSemanas.FirstOrDefaultAsync(x => x.IdDia == request.UpdateDto.IdDia);

            if (DiaSemana != null)
            {
                _mapper?.Map(request.UpdateDto, DiaSemana);
                _context.DiaSemanas.Update(DiaSemana);
                await _context.SaveChangesAsync();
            }

            var DiaSemanaDto = _mapper?.Map<GetDiaSemanaDto>(DiaSemana);
            if (DiaSemanaDto != null) response.UpdateData(DiaSemanaDto);

            response.AddOkResult(AppMessages.UpdateSuccess);

            return await Task.FromResult(response);
        }
    }
}
