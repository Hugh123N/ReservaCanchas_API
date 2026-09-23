using Reserva.Domain.Queries.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Reserva;

namespace Reserva.Domain.Queries.Dbo.Reserva
{
    /// <summary>
    /// Query para buscar reservas del cliente con paginación y filtros
    /// </summary>
    public class SearchReservasClienteQuery : SearchQueryBase<SearchReservaClienteFilterDto, ReservaClienteDto>
    {
        public int IdCliente { get; set; }

        public SearchReservasClienteQuery(int idCliente, SearchParamsDto<SearchReservaClienteFilterDto> searchParams)
            : base(searchParams)
        {
            IdCliente = idCliente;
        }
    }
}
