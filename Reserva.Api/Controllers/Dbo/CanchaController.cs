using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Dto.Dbo.Cancha;
using Reserva.Domain.Commands.Dbo.ImagenCancha;
using Reserva.Dto.Dbo.ImagenCancha;

namespace Reserva.Api.Controllers.Dbo
{
    [ApiController]
    [Route("api/Cancha")]
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
        public async Task<ResponseDto<GetCanchaDto>> Create(CreateCanchaDto createDto)
            => await _CanchaApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetCanchaDto>> Update(UpdateCanchaDto updateDto)
            => await _CanchaApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _CanchaApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetCanchaDto>> Get(int id)
            => await _CanchaApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListCanchaDto>>> List(int id)
            => await _CanchaApplication.List(id);
        [HttpPost("search")]
        public async Task<ResponseDto<SearchResultDto<SearchCanchaDto>>> Search(SearchParamsDto<SearchCanchaFilterDto> searchParams)
            => await _CanchaApplication.Search(searchParams);
        [HttpGet("selectcombo")]
        public async Task<ResponseDto<IEnumerable<SelectComboCanchaDto>>> SelectCombo()
            => await _CanchaApplication.SelectCombo();
        [HttpPost("select")]
        public async Task<ResponseDto<SearchResultDto<SelectCanchaDto>>> Select(SearchParamsDto<SelectCanchaFilterDto> searchParams)
            => await _CanchaApplication.Select(searchParams);

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
