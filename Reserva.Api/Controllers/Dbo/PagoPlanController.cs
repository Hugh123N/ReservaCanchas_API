using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Dto.Dbo.PagoPlan;

namespace Reserva.Api.Controllers.Dbo
{
    [ApiController]
    [Route("api/PagoPlan")]
    public class PagoPlanController : IPagoPlanApplication
    {
        private readonly IPagoPlanApplication _PagoPlanApplication;

        public PagoPlanController(IPagoPlanApplication PagoPlanApplication)
            => _PagoPlanApplication = PagoPlanApplication;

        [HttpPost]
        public async Task<ResponseDto<GetPagoPlanDto>> Create(CreatePagoPlanDto createDto)
            => await _PagoPlanApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetPagoPlanDto>> Update(UpdatePagoPlanDto updateDto)
            => await _PagoPlanApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _PagoPlanApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetPagoPlanDto>> Get(int id)
            => await _PagoPlanApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListPagoPlanDto>>> List(int id)
            => await _PagoPlanApplication.List(id);
        [HttpPost("search")]
        public async Task<ResponseDto<SearchResultDto<SearchPagoPlanDto>>> Search(SearchParamsDto<SearchPagoPlanFilterDto> searchParams)
            => await _PagoPlanApplication.Search(searchParams);
        [HttpGet("payments/{idProveedor}")]
        public async Task<ResponseDto<List<GetPagoPlanDto>>> GetPayments(int idProveedor)
            => await _PagoPlanApplication.GetPayments(idProveedor);
    }
}
