using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Usuario;
using Reserva.Dto.User;

namespace Reserva.Application.Abstractions.Dbo
{
    public interface IUsuarioApplication
    {
        Task<ResponseDto<GetUsuarioDto>> Create(CreateUsuarioDto createDto);
        Task<ResponseDto<GetUsuarioDto>> Update(UpdateUsuarioDto updateDto);
        Task<ResponseDto> Delete(Guid id);
        Task<ResponseDto<GetUsuarioDto>> Get(Guid id);
        Task<ResponseDto<IEnumerable<ListUsuarioDto>>> List(Guid id);
        Task<ResponseDto<SearchResultDto<SearchUsuarioDto>>> Search(SearchParamsDto<SearchUsuarioFilterDto> searchParams);
        Task<ResponseDto<IEnumerable<SelectComboUsuarioDto>>> SelectCombo();
        Task<ResponseDto<SearchResultDto<SelectUsuarioDto>>> Select(SearchParamsDto<SelectUsuarioFilterDto> searchParams);
        Task<ResponseDto<LoginResultDto>> Login(LoginDto loginDto);
        Task<ResponseDto<LoginResultDto>> CreateAndLogin(CreateAndLoginDto createDto);
        Task<ResponseDto<GetUsuarioDto>> CreateProveedor(CreateUsuarioProveedorDto createDto);
        Task<ResponseDto<GetUsuarioDto>> UpgradeToProveedor(Guid userId, UpgradeToProveedorDto upgradeDto);
        Task<ResponseDto> ForgotPassword(string email, string host);
        Task<ResponseDto> ResetPassword(ResetPasswordDto resetPasswordDto);
    }
}

