using MediatR;
using Microsoft.AspNetCore.Http;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Services.Canchas;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Cancha;

namespace Reserva.Domain.Commands.Dbo.Cancha;

public class ActualizarCanchaCompletaCommand(int id, string datos, IFormFileCollection? archivos) : CommandBase<GetCanchaDto>
{
    public int Id { get; } = id;
    public string Datos { get; } = datos;
    public IFormFileCollection? Archivos { get; } = archivos;
}

public class ActualizarCanchaCompletaCommandHandler(CanchaCompletaService service)
    : IRequestHandler<ActualizarCanchaCompletaCommand, ResponseDto<GetCanchaDto>>
{
    public Task<ResponseDto<GetCanchaDto>> Handle(ActualizarCanchaCompletaCommand request, CancellationToken ct)
        => service.ActualizarAsync(request.Id, request.Datos, request.Archivos, ct);
}
