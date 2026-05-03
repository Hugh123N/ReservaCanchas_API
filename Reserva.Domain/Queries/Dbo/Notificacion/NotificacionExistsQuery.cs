using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Notificacion;

namespace Reserva.Domain.Queries.Dbo.Notificacion
{
    public class NotificacionExistsQuery : QueryBase<bool>
    {
        public string? Modulo { get; set; }
        public string? Tipo { get; set; }
        public string? EntidadTipo { get; set; }
        public string? EntidadId { get; set; }

        public NotificacionExistsQuery(
            string? modulo = null,
            string? tipo = null,
            string? entidadTipo = null,
            string? entidadId = null)
        {
            Modulo = modulo;
            Tipo = tipo;
            EntidadTipo = entidadTipo;
            EntidadId = entidadId;
        }
    }
}
