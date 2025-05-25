using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Dto.Cancha.Usuario;

namespace Reserva.Api.Controllers.Cancha
{
    [ApiController]
    [Route("api/Usuario")]
    public class UsuarioController : IUsuarioApplication
    {
        private readonly IUsuarioApplication _UsuarioApplication;

        public UsuarioController(IUsuarioApplication UsuarioApplication)
            => _UsuarioApplication = UsuarioApplication;

        [HttpPost]
        public async Task<ResponseDto<GetUsuarioDto>> Create(CreateUsuarioDto createDto)
            => await _UsuarioApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetUsuarioDto>> Update(UpdateUsuarioDto updateDto)
            => await _UsuarioApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _UsuarioApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetUsuarioDto>> Get(int id)
            => await _UsuarioApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListUsuarioDto>>> List(int id)
            => await _UsuarioApplication.List(id);
        [HttpPost("search")]
        public async Task<ResponseDto<SearchResultDto<SearchUsuarioDto>>> Search(SearchParamsDto<SearchUsuarioFilterDto> searchParams)
            => await _UsuarioApplication.Search(searchParams);
        [HttpGet("selectcombo")]
        public async Task<ResponseDto<IEnumerable<SelectComboUsuarioDto>>> SelectCombo()
            => await _UsuarioApplication.SelectCombo();
        [HttpPost("select")]
        public async Task<ResponseDto<SearchResultDto<SelectUsuarioDto>>> Select(SearchParamsDto<SelectUsuarioFilterDto> searchParams)
            => await _UsuarioApplication.Select(searchParams);

    }
}
