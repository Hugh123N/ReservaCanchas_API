using Microsoft.AspNetCore.Mvc;
using Reserva.Common;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Dto.Dbo.Pago;

namespace Reserva.Api.Controllers.Dbo
{
    [ApiController]
    [Route("api/Pago")]
    public class PagoController : IPagoApplication
    {
        [Security.Authorize(Roles = Constants.Role.Proveedor + "," + Constants.Role.Operador)]
        [HttpGet("reserva/{idReserva:int}/movimientos")]
        public Task<ResponseDto<GetHistorialPagoDto>> HistorialReserva(int idReserva) => _PagoApplication.HistorialReserva(idReserva);

        [Security.Authorize(Roles = Constants.Role.Proveedor + "," + Constants.Role.Operador)]
        [HttpGet("{id:int}/movimientos")]
        public Task<ResponseDto<GetHistorialPagoDto>> Historial(int id) => _PagoApplication.Historial(id);

        [Security.Authorize(Roles = Constants.Role.Proveedor + "," + Constants.Role.Operador)]
        [HttpPost("registrar-cobro")]
        public Task<ResponseDto<GetHistorialPagoDto>> RegistrarCobro([FromBody] RegistrarMovimientoPagoDto dto)
            => _PagoApplication.RegistrarCobro(dto);

        [Security.Authorize(Roles = Constants.Role.Proveedor)]
        [HttpPost("corregir-registro")]
        public Task<ResponseDto<GetHistorialPagoDto>> CorregirRegistro([FromBody] RegistrarMovimientoPagoDto dto)
            => _PagoApplication.CorregirRegistro(dto);
        private readonly IPagoApplication _PagoApplication;

        public PagoController(IPagoApplication PagoApplication)
            => _PagoApplication = PagoApplication;

        [HttpPost]
        public async Task<ResponseDto<GetPagoDto>> Create(CreatePagoDto createDto)
            => await _PagoApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetPagoDto>> Update(UpdatePagoDto updateDto)
            => await _PagoApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _PagoApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetPagoDto>> Get(int id)
            => await _PagoApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListPagoDto>>> List(int id)
            => await _PagoApplication.List(id);
        [HttpPost("search")]
        public async Task<ResponseDto<SearchResultDto<SearchPagoDto>>> Search(SearchParamsDto<SearchPagoFilterDto> searchParams)
            => await _PagoApplication.Search(searchParams);
        [HttpGet("selectcombo")]
        public async Task<ResponseDto<IEnumerable<SelectComboPagoDto>>> SelectCombo()
            => await _PagoApplication.SelectCombo();
        [HttpPost("select")]
        public async Task<ResponseDto<SearchResultDto<SelectPagoDto>>> Select(SearchParamsDto<SelectPagoFilterDto> searchParams)
            => await _PagoApplication.Select(searchParams);

        [HttpPost("confirmar")]
        public async Task<ResponseDto<GetPagoDto>> ConfirmarPago([FromBody] ConfirmarPagoDto confirmarDto)
            => await _PagoApplication.ConfirmarPago(confirmarDto);

        [HttpPost("completar-pago")]
        public async Task<ResponseDto<GetPagoDto>> CompletarPago([FromBody] CompletarPagoDto completarDto)
            => await _PagoApplication.CompletarPago(completarDto);
    }
}
