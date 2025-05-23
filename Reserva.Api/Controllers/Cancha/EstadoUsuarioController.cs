using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Dto.Cancha.EstadoUsuario;

namespace Reserva.Apis.Controllers.Cancha
{
    [ApiController]
    [Route("api/EstadoUsuario")]
    public class EstadoUsuarioController : IEstadoUsuarioApplication
    {
        private readonly IEstadoUsuarioApplication _EstadoUsuarioApplication;

        public EstadoUsuarioController(IEstadoUsuarioApplication EstadoUsuarioApplication)
            => _EstadoUsuarioApplication = EstadoUsuarioApplication;

        [HttpPost]
        public async Task<ResponseDto<GetEstadoUsuarioDto>> Create(CreateEstadoUsuarioDto createDto)
            => await _EstadoUsuarioApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetEstadoUsuarioDto>> Update(UpdateEstadoUsuarioDto updateDto)
            => await _EstadoUsuarioApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _EstadoUsuarioApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetEstadoUsuarioDto>> Get(int id)
            => await _EstadoUsuarioApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListEstadoUsuarioDto>>> List(int id)
            => await _EstadoUsuarioApplication.List(id);
        [HttpPost("search")]
        public async Task<ResponseDto<SearchResultDto<SearchEstadoUsuarioDto>>> Search(SearchParamsDto<SearchEstadoUsuarioFilterDto> searchParams)
            => await _EstadoUsuarioApplication.Search(searchParams);
        [HttpGet("selectcombo")]
        public async Task<ResponseDto<IEnumerable<SelectComboEstadoUsuarioDto>>> SelectCombo()
            => await _EstadoUsuarioApplication.SelectCombo();
        [HttpPost("select")]
        public async Task<ResponseDto<SearchResultDto<SelectEstadoUsuarioDto>>> Select(SearchParamsDto<SelectEstadoUsuarioFilterDto> searchParams)
            => await _EstadoUsuarioApplication.Select(searchParams);

    }
}
