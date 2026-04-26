using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Dto.Dbo.ComprobantePagoPlan;

namespace Reserva.Apis.Controllers.Dbo
{
    [ApiController]
    [Route("api/ComprobantePagoPlan")]
    public class ComprobantePagoPlanController : IComprobantePagoPlanApplication
    {
        private readonly IComprobantePagoPlanApplication _ComprobantePagoPlanApplication;

        public ComprobantePagoPlanController(IComprobantePagoPlanApplication ComprobantePagoPlanApplication)
            => _ComprobantePagoPlanApplication = ComprobantePagoPlanApplication;

        [HttpPost]
        public async Task<ResponseDto<GetComprobantePagoPlanDto>> Create(CreateComprobantePagoPlanDto createDto)
            => await _ComprobantePagoPlanApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetComprobantePagoPlanDto>> Update(UpdateComprobantePagoPlanDto updateDto)
            => await _ComprobantePagoPlanApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _ComprobantePagoPlanApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetComprobantePagoPlanDto>> Get(int id)
            => await _ComprobantePagoPlanApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListComprobantePagoPlanDto>>> List(int id)
            => await _ComprobantePagoPlanApplication.List(id);
        [HttpPost("search")]
        public async Task<ResponseDto<SearchResultDto<SearchComprobantePagoPlanDto>>> Search(SearchParamsDto<SearchComprobantePagoPlanFilterDto> searchParams)
            => await _ComprobantePagoPlanApplication.Search(searchParams);

    }
}
