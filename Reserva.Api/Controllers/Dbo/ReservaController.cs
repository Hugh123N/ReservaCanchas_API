using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Dto.Dbo.Reserva;

namespace Reserva.Api.Controllers.Dbo
{
    [ApiController]
    [Route("api/Reserva")]
    public class ReservaController : IReservaApplication
    {
        private readonly IReservaApplication _ReservaApplication;

        public ReservaController(IReservaApplication ReservaApplication)
            => _ReservaApplication = ReservaApplication;

        [HttpPost]
        public async Task<ResponseDto<ReservaConPagoDto>> Create(CreateReservaDto createDto)
            => await _ReservaApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetReservaDto>> Update(UpdateReservaDto updateDto)
            => await _ReservaApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _ReservaApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetReservaDto>> Get(int id)
            => await _ReservaApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListReservaDto>>> List(int id)
            => await _ReservaApplication.List(id);
        [HttpPost("search")]
        public async Task<ResponseDto<SearchResultDto<SearchReservaDto>>> Search(SearchParamsDto<SearchReservaFilterDto> searchParams)
            => await _ReservaApplication.Search(searchParams);
        [HttpGet("selectcombo")]
        public async Task<ResponseDto<IEnumerable<SelectComboReservaDto>>> SelectCombo()
            => await _ReservaApplication.SelectCombo();
        [HttpPost("select")]
        public async Task<ResponseDto<SearchResultDto<SelectReservaDto>>> Select(SearchParamsDto<SelectReservaFilterDto> searchParams)
            => await _ReservaApplication.Select(searchParams);

        /// <summary>
        /// Endpoint para que el operador confirme una reserva pendiente y registre el pago
        /// </summary>
        [HttpPost("confirmar-reserva-operador")]
        public async Task<ResponseDto<GetReservaDto>> ConfirmarReservaOperador([FromBody] ConfirmarReservaOperadorDto confirmarDto)
            => await _ReservaApplication.ConfirmarReservaOperador(confirmarDto);

        /// <summary>
        /// Endpoint para que el operador libere/cancele una reserva pendiente
        /// </summary>
        [HttpPost("liberar-reserva-operador")]
        public async Task<ResponseDto<GetReservaDto>> LiberarReservaOperador([FromBody] LiberarReservaOperadorDto liberarDto)
            => await _ReservaApplication.LiberarReservaOperador(liberarDto);

        /// <summary>
        /// Endpoint para obtener todas las reservas pendientes de un proveedor
        /// </summary>
        [HttpGet("pendientes-operador/{idProveedor}")]
        public async Task<ResponseDto<IEnumerable<ReservaPendienteOperadorDto>>> ObtenerReservasPendientesOperador(int idProveedor)
            => await _ReservaApplication.ObtenerReservasPendientesOperador(idProveedor);

        /// <summary>
        /// Endpoint para que el cliente obtenga todas sus reservas (historial completo)
        /// </summary>
        [HttpGet("mis-reservas/{idUsuario}")]
        public async Task<ResponseDto<IEnumerable<ReservaClienteDto>>> ObtenerReservasCliente(Guid idUsuario)
            => await _ReservaApplication.ObtenerReservasCliente(idUsuario);
    }
}
