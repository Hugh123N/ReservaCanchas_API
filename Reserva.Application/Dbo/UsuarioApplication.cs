using MediatR;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Dbo.Usuario;
using Reserva.Domain.Commands.User;
using Reserva.Domain.Queries.Dbo.Usuario;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Token;
using Reserva.Dto.Dbo.Usuario;
using Reserva.Dto.User;

namespace Reserva.Application.Dbo
{
    public class UsuarioApplication : ApplicationBase, IUsuarioApplication
    {
        public UsuarioApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetUsuarioDto>> Create(CreateUsuarioDto createDto)
            => await _mediator.Send(new CreateUsuarioCommand(createDto));
        public async  Task<ResponseDto<GetUsuarioDto>>CreateProveedor(CreateUsuarioProveedorDto createDto)
            => await _mediator.Send(new CreateUsuarioProveedorCommand(createDto));
        public async Task<ResponseDto<GetUsuarioDto>> UpgradeToProveedor(Guid userId, UpgradeToProveedorDto upgradeDto)
            => await _mediator.Send(new UpgradeToProveedorCommand(userId, upgradeDto));
        public async Task<ResponseDto<GetUsuarioDto>> Update(UpdateUsuarioDto updateDto)
            => await _mediator.Send(new UpdateUsuarioCommand(updateDto));
        public async Task<ResponseDto> Delete(Guid id)
            => await _mediator.Send(new DeleteUsuarioCommand(id));
        public async Task<ResponseDto<GetUsuarioDto>> Get(Guid id)
            => await _mediator.Send(new GetUsuarioQuery(id));
        public async Task<ResponseDto<IEnumerable<ListUsuarioDto>>> List(Guid id)
            => await _mediator.Send(new ListUsuarioQuery(id));
        public async Task<ResponseDto<SearchResultDto<SearchUsuarioDto>>> Search(SearchParamsDto<SearchUsuarioFilterDto> searchParams)
            => await _mediator.Send(new SearchUsuarioQuery(searchParams));
        public async Task<ResponseDto<IEnumerable<SelectComboUsuarioDto>>> SelectCombo()
            => await _mediator.Send(new SelectComboUsuarioQuery());
        public async Task<ResponseDto<SearchResultDto<SelectUsuarioDto>>> Select(SearchParamsDto<SelectUsuarioFilterDto> searchParams)
             => await _mediator.Send(new SelectUsuarioQuery(searchParams));
        public async Task<ResponseDto<LoginResultDto>> Login(LoginDto loginDto)
            => await _mediator.Send(new LoginCommand(loginDto));
        public async Task<ResponseDto<AccessTokenDto>> RenewSession()
            => await _mediator.Send(new RenewSessionCommand());
        public async Task<ResponseDto<LoginResultDto>> CreateAndLogin(CreateAndLoginDto createDto)
            => await _mediator.Send(new CreateAndLoginCommand(createDto));
        public async Task<ResponseDto> ForgotPassword(string email, string host)
            => await _mediator.Send(new ForgotPasswordCommand(email, host));
        public async Task<ResponseDto> ResetPassword(ResetPasswordDto resetPasswordDto)
            => await _mediator.Send(new ResetPasswordCommand(resetPasswordDto));
        public async Task<ResponseDto> UpdateTelefono(string idUsuario, string telefono)
            => await _mediator.Send(new UpdateTelefonoCommand(idUsuario, telefono));
    }
}
