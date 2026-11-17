using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Dto.Dbo.Reserva;
using Reserva.Common;

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
        [Security.Authorize(Roles = Constants.Role.Proveedor + "," + Constants.Role.Operador)]
        public async Task<ResponseDto<GetReservaDto>> Update(UpdateReservaDto updateDto)
            => await _ReservaApplication.Update(updateDto);
        [HttpDelete("{id}")]
        [Security.Authorize(Roles = Constants.Role.Proveedor + "," + Constants.Role.Operador)]
        public async Task<ResponseDto> Delete(int id)
            => await _ReservaApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetReservaDto>> Get(int id)
            => await _ReservaApplication.Get(id);
        [HttpGet("gestion/{id:int}")]
        [Security.Authorize(Roles = Constants.Role.Proveedor + "," + Constants.Role.Operador)]
        public Task<ResponseDto<GetReservaDto>> GetGestion(int id) => _ReservaApplication.GetGestion(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListReservaDto>>> List(int id)
            => await _ReservaApplication.List(id);
        [HttpPost("search")]
        [Security.Authorize(Roles = Constants.Role.Proveedor + "," + Constants.Role.Operador)]
        public async Task<ResponseDto<SearchReservasResultDto>> Search(SearchParamsDto<SearchReservaFilterDto> searchParams)
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
        [Security.Authorize(Roles = Constants.Role.Proveedor + "," + Constants.Role.Operador)]
        public async Task<ResponseDto<GetReservaDto>> ConfirmarReservaOperador([FromBody] ConfirmarReservaOperadorDto confirmarDto)
            => await _ReservaApplication.ConfirmarReservaOperador(confirmarDto);

        /// <summary>
        /// Endpoint para que el operador libere/cancele una reserva pendiente
        /// </summary>
        [HttpPost("liberar-reserva-operador")]
        [Security.Authorize(Roles = Constants.Role.Proveedor + "," + Constants.Role.Operador)]
        public async Task<ResponseDto<GetReservaDto>> LiberarReservaOperador([FromBody] LiberarReservaOperadorDto liberarDto)
            => await _ReservaApplication.LiberarReservaOperador(liberarDto);

        /// <summary>
        /// Endpoint para obtener todas las reservas pendientes de un proveedor
        /// </summary>
        [HttpGet("pendientes-operador/{idProveedor}")]
        public async Task<ResponseDto<IEnumerable<ReservaPendienteOperadorDto>>> ObtenerReservasPendientesOperador(int idProveedor)
            => await _ReservaApplication.ObtenerReservasPendientesOperador(idProveedor);

        /// <summary>
        /// Endpoint para que el cliente busque sus reservas con paginación y filtros
        /// </summary>
        [HttpPost("mis-reservas/{idCliente}")]
        public async Task<ResponseDto<SearchResultDto<ReservaClienteDto>>> SearchReservasCliente(int idCliente,SearchParamsDto<SearchReservaClienteFilterDto> searchParams)
            => await _ReservaApplication.SearchReservasCliente(idCliente, searchParams);
    }
}
