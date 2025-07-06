using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Dto.Cancha.CanchaFavorita;

namespace Reserva.Apis.Controllers.Cancha
{
    [ApiController]
    [Route("api/CanchaFavorita")]
    public class CanchaFavoritaController : ICanchaFavoritaApplication
    {
        private readonly ICanchaFavoritaApplication _CanchaFavoritaApplication;

        public CanchaFavoritaController(ICanchaFavoritaApplication CanchaFavoritaApplication)
            => _CanchaFavoritaApplication = CanchaFavoritaApplication;

        [HttpPost]
        public async Task<ResponseDto<GetCanchaFavoritaDto>> Create(CreateCanchaFavoritaDto createDto)
            => await _CanchaFavoritaApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetCanchaFavoritaDto>> Update(UpdateCanchaFavoritaDto updateDto)
            => await _CanchaFavoritaApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _CanchaFavoritaApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetCanchaFavoritaDto>> Get(int id)
            => await _CanchaFavoritaApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListCanchaFavoritaDto>>> List(int id)
            => await _CanchaFavoritaApplication.List(id);
        [HttpPost("search")]
        public async Task<ResponseDto<SearchResultDto<SearchCanchaFavoritaDto>>> Search(SearchParamsDto<SearchCanchaFavoritaFilterDto> searchParams)
            => await _CanchaFavoritaApplication.Search(searchParams);
        [HttpGet("selectcombo")]
        public async Task<ResponseDto<IEnumerable<SelectComboCanchaFavoritaDto>>> SelectCombo()
            => await _CanchaFavoritaApplication.SelectCombo();
        [HttpPost("select")]
        public async Task<ResponseDto<SearchResultDto<SelectCanchaFavoritaDto>>> Select(SearchParamsDto<SelectCanchaFavoritaFilterDto> searchParams)
            => await _CanchaFavoritaApplication.Select(searchParams);

    }
}
