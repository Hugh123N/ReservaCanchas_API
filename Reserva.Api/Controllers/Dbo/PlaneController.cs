using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Dto.Dbo.Plane;

namespace Reserva.Api.Controllers.Dbo
{
    [ApiController]
    [Route("api/Plane")]
    public class PlaneController : IPlaneApplication
    {
        private readonly IPlaneApplication _PlaneApplication;

        public PlaneController(IPlaneApplication PlaneApplication)
            => _PlaneApplication = PlaneApplication;

        [HttpPost]
        public async Task<ResponseDto<GetPlaneDto>> Create(CreatePlaneDto createDto)
            => await _PlaneApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetPlaneDto>> Update(UpdatePlaneDto updateDto)
            => await _PlaneApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _PlaneApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetPlaneDto>> Get(int id)
            => await _PlaneApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListPlaneDto>>> List(int id)
            => await _PlaneApplication.List(id);

    }
}
