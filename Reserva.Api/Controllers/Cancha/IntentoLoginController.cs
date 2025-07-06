using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Dto.Cancha.IntentoLogin;

namespace Reserva.Apis.Controllers.Cancha
{
    [ApiController]
    [Route("api/IntentoLogin")]
    public class IntentoLoginController : IIntentoLoginApplication
    {
        private readonly IIntentoLoginApplication _IntentoLoginApplication;

        public IntentoLoginController(IIntentoLoginApplication IntentoLoginApplication)
            => _IntentoLoginApplication = IntentoLoginApplication;

        [HttpPost]
        public async Task<ResponseDto<GetIntentoLoginDto>> Create(CreateIntentoLoginDto createDto)
            => await _IntentoLoginApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetIntentoLoginDto>> Update(UpdateIntentoLoginDto updateDto)
            => await _IntentoLoginApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _IntentoLoginApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetIntentoLoginDto>> Get(int id)
            => await _IntentoLoginApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListIntentoLoginDto>>> List(int id)
            => await _IntentoLoginApplication.List(id);
        [HttpPost("search")]
        public async Task<ResponseDto<SearchResultDto<SearchIntentoLoginDto>>> Search(SearchParamsDto<SearchIntentoLoginFilterDto> searchParams)
            => await _IntentoLoginApplication.Search(searchParams);
        [HttpPost("select")]
        public async Task<ResponseDto<SearchResultDto<SelectIntentoLoginDto>>> Select(SearchParamsDto<SelectIntentoLoginFilterDto> searchParams)
            => await _IntentoLoginApplication.Select(searchParams);

    }
}
