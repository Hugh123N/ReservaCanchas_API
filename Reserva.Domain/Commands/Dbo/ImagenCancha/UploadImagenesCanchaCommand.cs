using Microsoft.AspNetCore.Http;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.ImagenCancha;

namespace Reserva.Domain.Commands.Dbo.ImagenCancha
{
    public class UploadImagenesCanchaCommand : CommandBase<List<ImagenCanchaDto>>
    {
        public int IdCancha { get; set; }
        public IFormFileCollection Files { get; set; } = null!;
        public int? IndicePrincipal { get; set; }
    }
}
