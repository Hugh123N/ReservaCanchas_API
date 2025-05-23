using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Dto.Cancha.EstadoProveedor;

namespace Reserva.Apis.Controllers.Cancha
{
    [ApiController]
    [Route("api/EstadoProveedor")]
    public class EstadoProveedorController : IEstadoProveedorApplication
    {
        private readonly IEstadoProveedorApplication _EstadoProveedorApplication;

        public EstadoProveedorController(IEstadoProveedorApplication EstadoProveedorApplication)
            => _EstadoProveedorApplication = EstadoProveedorApplication;

        [HttpPost]
        public async Task<ResponseDto<GetEstadoProveedorDto>> Create(CreateEstadoProveedorDto createDto)
            => await _EstadoProveedorApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetEstadoProveedorDto>> Update(UpdateEstadoProveedorDto updateDto)
            => await _EstadoProveedorApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _EstadoProveedorApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetEstadoProveedorDto>> Get(int id)
            => await _EstadoProveedorApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListEstadoProveedorDto>>> List(int id)
            => await _EstadoProveedorApplication.List(id);
        [HttpPost("search")]
        public async Task<ResponseDto<SearchResultDto<SearchEstadoProveedorDto>>> Search(SearchParamsDto<SearchEstadoProveedorFilterDto> searchParams)
            => await _EstadoProveedorApplication.Search(searchParams);
        [HttpGet("selectcombo")]
        public async Task<ResponseDto<IEnumerable<SelectComboEstadoProveedorDto>>> SelectCombo()
            => await _EstadoProveedorApplication.SelectCombo();
        [HttpPost("select")]
        public async Task<ResponseDto<SearchResultDto<SelectEstadoProveedorDto>>> Select(SearchParamsDto<SelectEstadoProveedorFilterDto> searchParams)
            => await _EstadoProveedorApplication.Select(searchParams);

    }
}
