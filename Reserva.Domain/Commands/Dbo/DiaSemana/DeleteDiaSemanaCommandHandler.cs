using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Entity.Models;
using Reserva.Entity.ITransactions;
using Microsoft.EntityFrameworkCore;

namespace Reserva.Domain.Commands.Dbo.DiaSemana
{
    public class DeleteDiaSemanaCommandHandler : CommandHandlerBase<DeleteDiaSemanaCommand>
    {
        private readonly ReservaCanchasContext _context;

        public DeleteDiaSemanaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteDiaSemanaCommandValidator validator,
            ReservaCanchasContext context
        ) : base(unitOfWork, mapper, validator)
        {
            _context = context;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteDiaSemanaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var DiaSemana = await _context.DiaSemanas.FindAsync(request.Id);

            if (DiaSemana != null)
            {
                _context.DiaSemanas.Remove(DiaSemana);
                await _context.SaveChangesAsync();
                response.AddOkResult(AppMessages.DeleteSuccess);
            }

            return response;
        }
    }
}
