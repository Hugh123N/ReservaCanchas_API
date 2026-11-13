using AutoMapper;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Pago;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.Pago;

public class ConfirmarPagoCommandHandler : CommandHandlerBase<ConfirmarPagoCommand, GetPagoDto>
{
    public ConfirmarPagoCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ConfirmarPagoCommandValidator validator)
        : base(unitOfWork, mapper, validator) { }

    public override Task<ResponseDto<GetPagoDto>> HandleCommand(ConfirmarPagoCommand request, CancellationToken cancellationToken)
    {
        var response = new ResponseDto<GetPagoDto>();
        response.AddErrorResult("Registra el dinero recibido mediante Registrar cobro, con el método y la referencia de la operación.");
        return Task.FromResult(response);
    }
}
