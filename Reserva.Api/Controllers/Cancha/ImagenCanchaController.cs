using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Dto.Cancha.ImagenCancha;

namespace Reserva.Api.Controllers.Cancha
{
    [ApiController]
    [Route("api/ImagenCancha")]
    public class ImagenCanchaController : IImagenCanchaApplication
    {
        private readonly IImagenCanchaApplication _ImagenCanchaApplication;

        public ImagenCanchaController(IImagenCanchaApplication ImagenCanchaApplication)
            => _ImagenCanchaApplication = ImagenCanchaApplication;

        [HttpPost]
        public async Task<ResponseDto<GetImagenCanchaDto>> Create(CreateImagenCanchaDto createDto)
            => await _ImagenCanchaApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetImagenCanchaDto>> Update(UpdateImagenCanchaDto updateDto)
            => await _ImagenCanchaApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _ImagenCanchaApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetImagenCanchaDto>> Get(int id)
            => await _ImagenCanchaApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListImagenCanchaDto>>> List(int id)
            => await _ImagenCanchaApplication.List(id);
        [HttpPost("search")]
        public async Task<ResponseDto<SearchResultDto<SearchImagenCanchaDto>>> Search(SearchParamsDto<SearchImagenCanchaFilterDto> searchParams)
            => await _ImagenCanchaApplication.Search(searchParams);
        [HttpGet("selectcombo")]
        public async Task<ResponseDto<IEnumerable<SelectComboImagenCanchaDto>>> SelectCombo()
            => await _ImagenCanchaApplication.SelectCombo();
        [HttpPost("select")]
        public async Task<ResponseDto<SearchResultDto<SelectImagenCanchaDto>>> Select(SearchParamsDto<SelectImagenCanchaFilterDto> searchParams)
            => await _ImagenCanchaApplication.Select(searchParams);

    }
}
