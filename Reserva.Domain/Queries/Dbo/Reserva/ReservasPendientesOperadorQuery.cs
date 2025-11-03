using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Reserva;

namespace Reserva.Domain.Queries.Dbo.Reserva
{
    public class ReservasPendientesOperadorQuery : QueryBase<IEnumerable<ReservaPendienteOperadorDto>>
    {
        /// <summary>
        /// ID del proveedor para filtrar las reservas de sus canchas
        /// </summary>
        public ReservasPendientesOperadorQuery(int idProveedor) => IdProveedor = idProveedor;

        public int IdProveedor { get; set; }
    }
}
