using MediatR;
using Microsoft.AspNetCore.Http;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Services.Canchas;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Cancha;

namespace Reserva.Domain.Commands.Dbo.Cancha;

public class CrearCanchaCompletaCommand(string datos, IFormFileCollection archivos) : CommandBase<GetCanchaDto>
{
    public string Datos { get; } = datos;
    public IFormFileCollection Archivos { get; } = archivos;
}

public class CrearCanchaCompletaCommandHandler(CanchaCompletaService service)
    : IRequestHandler<CrearCanchaCompletaCommand, ResponseDto<GetCanchaDto>>
{
    public Task<ResponseDto<GetCanchaDto>> Handle(CrearCanchaCompletaCommand request, CancellationToken ct)
        => service.CrearAsync(request.Datos, request.Archivos, ct);
}
