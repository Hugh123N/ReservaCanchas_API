using AutoMapper;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Pago;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.Pago;

public class CompletarPagoCommandHandler : CommandHandlerBase<CompletarPagoCommand, GetPagoDto>
{
    public CompletarPagoCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, CompletarPagoCommandValidator validator)
        : base(unitOfWork, mapper, validator) { }

    public override Task<ResponseDto<GetPagoDto>> HandleCommand(CompletarPagoCommand request, CancellationToken cancellationToken)
    {
        var response = new ResponseDto<GetPagoDto>();
        response.AddErrorResult("Completa el saldo mediante Registrar cobro, indicando el método de esta operación.");
        return Task.FromResult(response);
    }
}
