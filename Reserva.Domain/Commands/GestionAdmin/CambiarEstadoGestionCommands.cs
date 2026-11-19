using Reserva.Domain.Commands.Base;
using Reserva.Domain.Services.GestionAdmin;
using Reserva.Dto.Base;
using Reserva.Dto.GestionAdmin;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.GestionAdmin;

public class CambiarProveedorAdminCommand(int id, CambiarEstadoGestionDto cambio) : CommandBase
{
    public int Id { get; } = id;
    public CambiarEstadoGestionDto Cambio { get; } = cambio;
}

public class CambiarCanchaAdminCommand(int id, CambiarEstadoGestionDto cambio) : CommandBase
{
    public int Id { get; } = id;
    public CambiarEstadoGestionDto Cambio { get; } = cambio;
}

public class ReenviarCanchaRevisionCommand(int id, CambiarEstadoGestionDto cambio) : CommandBase
{
    public int Id { get; } = id;
    public CambiarEstadoGestionDto Cambio { get; } = cambio;
}

public class CambiarProveedorAdminCommandHandler(IUnitOfWork unitOfWork, GestionAdminService service) : CommandHandlerBase<CambiarProveedorAdminCommand>(unitOfWork)
{
    public override Task<ResponseDto> HandleCommand(CambiarProveedorAdminCommand request, CancellationToken ct)
        => service.CambiarProveedorAsync(request.Id, request.Cambio);
}

public class CambiarCanchaAdminCommandHandler(IUnitOfWork unitOfWork, GestionAdminService service) : CommandHandlerBase<CambiarCanchaAdminCommand>(unitOfWork)
{
    public override Task<ResponseDto> HandleCommand(CambiarCanchaAdminCommand request, CancellationToken ct)
        => service.CambiarCanchaAsync(request.Id, request.Cambio);
}

public class ReenviarCanchaRevisionCommandHandler(IUnitOfWork unitOfWork, GestionAdminService service) : CommandHandlerBase<ReenviarCanchaRevisionCommand>(unitOfWork)
{
    public override Task<ResponseDto> HandleCommand(ReenviarCanchaRevisionCommand request, CancellationToken ct)
        => service.CambiarCanchaAsync(request.Id, request.Cambio, true);
}
