using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Dto.Dbo.Cliente;

namespace Reserva.Api.Controllers.Dbo
{
    [ApiController]
    [Route("api/Cliente")]
    [Security.Authorize]
    public class ClienteController : IClienteApplication
    {
        private readonly IClienteApplication _ClienteApplication;

        public ClienteController(IClienteApplication ClienteApplication)
            => _ClienteApplication = ClienteApplication;

        [AllowAnonymous]
        [HttpPost]
        public async Task<ResponseDto<GetClienteDto>> Create(CreateClienteRegistroDto createDto)
            => await _ClienteApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetClienteDto>> Update(UpdateClienteDto updateDto)
            => await _ClienteApplication.Update(updateDto);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetClienteDto>> Get(int id)
            => await _ClienteApplication.Get(id);
    }
}
