using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Dto.Dbo.Ubigeo;

namespace Reserva.Api.Controllers.Dbo
{
    [ApiController]
    [Route("api/Ubigeo")]
    //[Security.Authorize]
    public class UbigeoController : IUbigeoApplication
    {
        private readonly IUbigeoApplication _UbigeoApplication;

        public UbigeoController(IUbigeoApplication UbigeoApplication)
            => _UbigeoApplication = UbigeoApplication;

        [HttpPost]
        public async Task<ResponseDto<GetUbigeoDto>> Create(CreateUbigeoDto createDto)
            => await _UbigeoApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetUbigeoDto>> Update(UpdateUbigeoDto updateDto)
            => await _UbigeoApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(string id)
            => await _UbigeoApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetUbigeoDto>> Get(string id)
            => await _UbigeoApplication.Get(id);
        [HttpGet("list")]
        public async Task<ResponseDto<IEnumerable<DepartamentoDto>>> List()
            => await _UbigeoApplication.List();
        [HttpGet("listAll")]
        public async Task<ResponseDto<IEnumerable<GetUbigeoDto>>> ListAll()
            => await _UbigeoApplication.ListAll();
        [HttpPost("select")]
        public async Task<ResponseDto<SearchResultDto<SelectUbigeoDto>>> Select(SearchParamsDto<SelectUbigeoFilterDto> searchParams)
            => await _UbigeoApplication.Select(searchParams);
        [HttpGet("buscar")]
        public async Task<ResponseDto<GetUbigeoDto>> Buscar(
            [FromQuery] string departamento,
            [FromQuery] string provincia,
            [FromQuery] string distrito)
            => await _UbigeoApplication.Buscar(departamento, provincia, distrito);

    }
}
