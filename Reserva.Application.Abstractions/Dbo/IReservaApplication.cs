using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Reserva;

namespace Reserva.Application.Abstractions.Dbo
{
    public interface IReservaApplication
    {
        /// <summary>
        /// Crea una nueva pre-reserva en estado PENDIENTE (solo efectivo)
        /// </summary>
        Task<ResponseDto<ReservaConPagoDto>> Create(CreateReservaDto createDto);
        Task<ResponseDto<GetReservaDto>> Update(UpdateReservaDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetReservaDto>> Get(int id);
        Task<ResponseDto<IEnumerable<ListReservaDto>>> List(int id);
        Task<ResponseDto<SearchResultDto<SearchReservaDto>>> Search(SearchParamsDto<SearchReservaFilterDto> searchParams);
        Task<ResponseDto<IEnumerable<SelectComboReservaDto>>> SelectCombo();
        Task<ResponseDto<SearchResultDto<SelectReservaDto>>> Select(SearchParamsDto<SelectReservaFilterDto> searchParams);

        // Operaciones para el Operador
        /// <summary>
        /// Confirma una reserva pendiente y registra el pago/adelanto
        /// </summary>
        Task<ResponseDto<GetReservaDto>> ConfirmarReservaOperador(ConfirmarReservaOperadorDto confirmarDto);

        /// <summary>
        /// Libera/cancela una reserva pendiente para que el horario quede disponible
        /// </summary>
        Task<ResponseDto<GetReservaDto>> LiberarReservaOperador(LiberarReservaOperadorDto liberarDto);

        /// <summary>
        /// Obtiene lista de reservas pendientes del proveedor
        /// </summary>
        Task<ResponseDto<IEnumerable<ReservaPendienteOperadorDto>>> ObtenerReservasPendientesOperador(int idProveedor);

        // Operaciones para el Cliente
        /// <summary>
        /// Busca reservas del cliente con paginación y filtros
        /// </summary>
        Task<ResponseDto<SearchResultDto<ReservaClienteDto>>> SearchReservasCliente(Guid idUsuario, SearchParamsDto<SearchReservaClienteFilterDto> searchParams);
    }
}

