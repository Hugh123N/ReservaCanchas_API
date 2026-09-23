using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Calendario;

namespace Reserva.Domain.Queries.Dbo.Calendario
{
    /// <summary>
    /// Query para buscar clientes por nombre o teléfono
    /// </summary>
    public class BuscarClienteQuery : QueryBase<List<ClientDto>>
    {
        /// <summary>
        /// Término de búsqueda (puede ser nombre, apellido o teléfono)
        /// </summary>
        public string TerminoBusqueda { get; set; } = string.Empty;
    }
}
