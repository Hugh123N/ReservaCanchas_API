using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Calendario;

namespace Reserva.Domain.Queries.Dbo.Calendario
{
    /// <summary>
    /// Consulta un cliente por DNI y completa sus datos con PersonLookup cuando aún no está registrado.
    /// </summary>
    public class BuscarClienteQuery : QueryBase<ClientDto>
    {
        /// <summary>
        /// DNI de ocho dígitos.
        /// </summary>
        public string Dni { get; set; } = string.Empty;
    }
}
