using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reserva.Api.Security;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Reclamacion;

namespace Reserva.Api.Controllers.Dbo
{
    [ApiController]
    [Route("api/Reclamacion")]
    [Reserva.Api.Security.Authorize]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class ReclamacionController : IReclamacionApplication
    {
        private readonly IReclamacionApplication _ReclamacionApplication;

        public ReclamacionController(IReclamacionApplication ReclamacionApplication)
            => _ReclamacionApplication = ReclamacionApplication;

        [AllowAnonymous]
        [HttpPost]
        [Consumes("application/json")]
        [RequestSizeLimit(32768)]
        [PlainTextPayload]
        public async Task<ResponseDto<GetReclamacionDto>> Create([FromBody] CreateReclamacionDto createDto, CancellationToken cancellationToken)
            => await _ReclamacionApplication.Create(createDto, cancellationToken);

        [HttpGet("{id:int:min(1)}")]
        [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ResponseDto<GetReclamacionDto>> Get(int id, CancellationToken cancellationToken)
            => await _ReclamacionApplication.Get(id, cancellationToken);

        [HttpPost("search")]
        [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ResponseDto<SearchResultDto<SearchReclamacionDto>>> Search(
            [FromBody] SearchParamsDto<SearchReclamacionFilterDto> searchParams, CancellationToken cancellationToken)
            => await _ReclamacionApplication.Search(searchParams, cancellationToken);

        [HttpPut("{id:int:min(1)}/atencion")]
        [Consumes("application/json")]
        [RequestSizeLimit(32768)]
        [PlainTextPayload]
        [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ResponseDto> Atender(int id, [FromBody] AtenderReclamacionDto atenderDto, CancellationToken cancellationToken)
            => await _ReclamacionApplication.Atender(id, atenderDto, cancellationToken);
    }
}
