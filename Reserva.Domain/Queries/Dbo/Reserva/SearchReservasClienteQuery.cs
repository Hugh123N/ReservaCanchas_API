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
        public Guid IdUsuario { get; set; }

        public SearchReservasClienteQuery(Guid idUsuario, SearchParamsDto<SearchReservaClienteFilterDto> searchParams)
            : base(searchParams)
        {
            IdUsuario = idUsuario;
        }
    }
}
