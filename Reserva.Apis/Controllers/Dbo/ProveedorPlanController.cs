using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Dto.Dbo.ProveedorPlan;

namespace Reserva.Apis.Controllers.Dbo
{
    [ApiController]
    [Route("api/ProveedorPlan")]
    public class ProveedorPlanController : IProveedorPlanApplication
    {
        private readonly IProveedorPlanApplication _ProveedorPlanApplication;

        public ProveedorPlanController(IProveedorPlanApplication ProveedorPlanApplication)
            => _ProveedorPlanApplication = ProveedorPlanApplication;

        [HttpPost]
        public async Task<ResponseDto<GetProveedorPlanDto>> Create(CreateProveedorPlanDto createDto)
            => await _ProveedorPlanApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetProveedorPlanDto>> Update(UpdateProveedorPlanDto updateDto)
            => await _ProveedorPlanApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _ProveedorPlanApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetProveedorPlanDto>> Get(int id)
            => await _ProveedorPlanApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListProveedorPlanDto>>> List(int id)
            => await _ProveedorPlanApplication.List(id);
        [HttpPost("search")]
        public async Task<ResponseDto<SearchResultDto<SearchProveedorPlanDto>>> Search(SearchParamsDto<SearchProveedorPlanFilterDto> searchParams)
            => await _ProveedorPlanApplication.Search(searchParams);

    }
}
