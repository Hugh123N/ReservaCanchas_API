using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Usuario;
using Reserva.Dto.User;

namespace Reserva.Api.Controllers.Cancha
{
    [ApiController]
    [Route("api/Usuario")]
    public class UsuarioController : IUsuarioApplication
    {
        private readonly IUsuarioApplication _UsuarioApplication;

        public UsuarioController(IUsuarioApplication UsuarioApplication)
            => _UsuarioApplication = UsuarioApplication;

        [HttpPost("register/cliente")]
        public async Task<ResponseDto<GetUsuarioDto>> Create(CreateUsuarioDto createDto)
            => await _UsuarioApplication.Create(createDto);
        [HttpPost("register/proveedor")]
        public async Task<ResponseDto<GetUsuarioDto>> CreateProveedor(CreateUsuarioProveedorDto createDto)
            => await _UsuarioApplication.CreateProveedor(createDto);
        [HttpPut("{userId}/upgrade-to-proveedor")]
        //[Authorize]
        public async Task<ResponseDto<GetUsuarioDto>> UpgradeToProveedor(Guid userId, [FromBody] UpgradeToProveedorDto upgradeDto)
            => await _UsuarioApplication.UpgradeToProveedor(userId, upgradeDto);
        [HttpPut]
        public async Task<ResponseDto<GetUsuarioDto>> Update(UpdateUsuarioDto updateDto)
            => await _UsuarioApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(Guid id)
            => await _UsuarioApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetUsuarioDto>> Get(Guid id)
            => await _UsuarioApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListUsuarioDto>>> List(Guid id)
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
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ResponseDto<LoginResultDto>> Login(LoginDto loginDto)
            => await _UsuarioApplication.Login(loginDto);
        [HttpPost("client/loginAndCreate")]
        public async Task<ResponseDto<LoginResultDto>> CreateAndLogin(CreateAndLoginDto createDto)
            => await _UsuarioApplication.CreateAndLogin(createDto);
        [AllowAnonymous]
        [HttpGet("forgot-password/{email}/{host}")]
        public async Task<ResponseDto> ForgotPassword(string email, string host)
            => await _UsuarioApplication.ForgotPassword(email, host);
        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<ResponseDto> ResetPassword(ResetPasswordDto resetPasswordDto)
            => await _UsuarioApplication.ResetPassword(resetPasswordDto);

    }
}
