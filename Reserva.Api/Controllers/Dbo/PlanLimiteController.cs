using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Dto.Dbo.PlanLimite;

namespace Reserva.Api.Controllers.Dbo
{
    [ApiController]
    [Route("api/PlanLimite")]
    public class PlanLimiteController : IPlanLimiteApplication
    {
        private readonly IPlanLimiteApplication _PlanLimiteApplication;

        public PlanLimiteController(IPlanLimiteApplication PlanLimiteApplication)
            => _PlanLimiteApplication = PlanLimiteApplication;

        [HttpPost]
        public async Task<ResponseDto<GetPlanLimiteDto>> Create(CreatePlanLimiteDto createDto)
            => await _PlanLimiteApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetPlanLimiteDto>> Update(UpdatePlanLimiteDto updateDto)
            => await _PlanLimiteApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _PlanLimiteApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetPlanLimiteDto>> Get(int id)
            => await _PlanLimiteApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListPlanLimiteDto>>> List(int id)
            => await _PlanLimiteApplication.List(id);

    }
}
