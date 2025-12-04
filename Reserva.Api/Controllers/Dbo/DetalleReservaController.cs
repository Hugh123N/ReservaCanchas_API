using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Dto.Dbo.DetalleReserva;

namespace Reserva.Api.Controllers.Dbo
{
    [ApiController]
    [Route("api/DetalleReserva")]
    public class DetalleReservaController : IDetalleReservaApplication
    {
        private readonly IDetalleReservaApplication _DetalleReservaApplication;

        public DetalleReservaController(IDetalleReservaApplication DetalleReservaApplication)
            => _DetalleReservaApplication = DetalleReservaApplication;

        [HttpPost]
        public async Task<ResponseDto<GetDetalleReservaDto>> Create(CreateDetalleReservaDto createDto)
            => await _DetalleReservaApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetDetalleReservaDto>> Update(UpdateDetalleReservaDto updateDto)
            => await _DetalleReservaApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _DetalleReservaApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetDetalleReservaDto>> Get(int id)
            => await _DetalleReservaApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListDetalleReservaDto>>> List(int id)
            => await _DetalleReservaApplication.List(id);
        
    }
}
