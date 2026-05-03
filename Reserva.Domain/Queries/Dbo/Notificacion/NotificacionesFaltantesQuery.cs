using System.Collections.Generic;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Base;

namespace Reserva.Domain.Queries.Dbo.Notificacion
{
    public class NotificacionesFaltantesQuery : QueryBase<List<string>>
    {
        public string Modulo { get; set; } = null!;
        public string Tipo { get; set; } = null!;
        public string EntidadTipo { get; set; } = null!;
        public List<string> EntidadIds { get; set; } = new();

        public NotificacionesFaltantesQuery(
            string modulo,
            string tipo,
            string entidadTipo,
            List<string> entidadIds)
        {
            Modulo = modulo;
            Tipo = tipo;
            EntidadTipo = entidadTipo;
            EntidadIds = entidadIds ?? new List<string>();
        }
    }
}
