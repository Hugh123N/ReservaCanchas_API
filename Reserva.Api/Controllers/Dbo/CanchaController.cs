using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Dto.Dbo.Cancha;
using Reserva.Domain.Commands.Dbo.ImagenCancha;
using Reserva.Dto.Dbo.ImagenCancha;
using Reserva.Api.Security;
using Microsoft.AspNetCore.Authorization;

namespace Reserva.Api.Controllers.Dbo
{
    [ApiController]
    [Route("api/Cancha")]
    [Security.Authorize]
    public class CanchaController : ICanchaApplication
    {
        private readonly ICanchaApplication _CanchaApplication;
        private readonly IMediator _mediator;

        public CanchaController(
            ICanchaApplication CanchaApplication,
            IMediator mediator)
        {
            _CanchaApplication = CanchaApplication;
            _mediator = mediator;
        }

        [HttpPost]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Proveedor")]
        public async Task<ResponseDto<GetCanchaDto>> Create(CreateCanchaDto createDto)
            => await _CanchaApplication.Create(createDto);
        [HttpPut]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Proveedor")]
        public async Task<ResponseDto<GetCanchaDto>> Update(UpdateCanchaDto updateDto)
            => await _CanchaApplication.Update(updateDto);
        [HttpDelete("{id}")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Proveedor")]
        public async Task<ResponseDto> Delete(int id)
            => await _CanchaApplication.Delete(id);
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetCanchaDto>> Get(int id)
            => await _CanchaApplication.Get(id);
        [HttpGet("gestion/{id:int}")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,Proveedor,Operador")]
        public Task<ResponseDto<GetCanchaDto>> GetGestion(int id) => _CanchaApplication.GetGestion(id);
        [HttpPost("gestion/search")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Proveedor")]
        public Task<ResponseDto<SearchResultDto<SearchCanchaDto>>> SearchGestion(SearchParamsDto<SearchCanchaFilterDto> searchParams)
            => _CanchaApplication.SearchGestion(searchParams);
        [HttpPost("list")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,Proveedor,Operador")]
        public async Task<ResponseDto<IEnumerable<ListCanchaDto>>> List(int id)
            => await _CanchaApplication.List(id);
        [AllowAnonymous]
        [HttpPost("search")]
        public async Task<ResponseDto<SearchResultDto<SearchCanchaDto>>> Search(SearchParamsDto<SearchCanchaFilterDto> searchParams)
            => await _CanchaApplication.Search(searchParams);
        [HttpGet("selectcombo")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,Proveedor,Operador")]
        public async Task<ResponseDto<IEnumerable<SelectComboCanchaDto>>> SelectCombo()
            => await _CanchaApplication.SelectCombo();
        [HttpPost("select")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,Proveedor,Operador")]
        public async Task<ResponseDto<SearchResultDto<SelectCanchaDto>>> Select(SearchParamsDto<SelectCanchaFilterDto> searchParams)
            => await _CanchaApplication.Select(searchParams);
        [HttpGet("config/{id}")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,Proveedor,Operador")]
        public async Task<ResponseDto<GetCanchaConfigDto>> GetConfig(int id)
            => await _CanchaApplication.GetConfig(id);

        [HttpPost("{idCancha}/imagenes")]
        [RequestSizeLimit(26_214_400)] // 25MB total
        public async Task<ResponseDto<List<ImagenCanchaDto>>> UploadImagenes(
            int idCancha,[FromForm] IFormFileCollection files,[FromForm] int? indicePrincipal)
        {
            var command = new UploadImagenesCanchaCommand
            {
                IdCancha = idCancha,
                Files = files,
                IndicePrincipal = indicePrincipal
            };
            return await _mediator.Send(command);
        }

        [HttpDelete("imagenes/{idImagen}")]
        public async Task<ResponseDto> DeleteImagen(int idImagen)
        {
            var command = new DeleteImagenCanchaCommand(idImagen);
            return await _mediator.Send(command);
        }
    }
}
