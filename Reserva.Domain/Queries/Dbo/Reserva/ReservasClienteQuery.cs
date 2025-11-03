using MediatR;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Reserva;

namespace Reserva.Domain.Queries.Dbo.Reserva
{
    /// <summary>
    /// Query para obtener todas las reservas de un cliente
    /// </summary>
    public class ReservasClienteQuery : IRequest<ResponseDto<IEnumerable<ReservaClienteDto>>>
    {
        public Guid IdUsuario { get; set; }

        public ReservasClienteQuery(Guid idUsuario)
        {
            IdUsuario = idUsuario;
        }
    }
}
