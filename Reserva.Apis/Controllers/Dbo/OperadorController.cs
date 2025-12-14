using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Dto.Dbo.Operador;

namespace Reserva.Apis.Controllers.Dbo
{
    [ApiController]
    [Route("api/Operador")]
    public class OperadorController : IOperadorApplication
    {
        private readonly IOperadorApplication _OperadorApplication;

        public OperadorController(IOperadorApplication OperadorApplication)
            => _OperadorApplication = OperadorApplication;

        [HttpPost]
        public async Task<ResponseDto<GetOperadorDto>> Create(CreateOperadorDto createDto)
            => await _OperadorApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetOperadorDto>> Update(UpdateOperadorDto updateDto)
            => await _OperadorApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _OperadorApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetOperadorDto>> Get(int id)
            => await _OperadorApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListOperadorDto>>> List(int id)
            => await _OperadorApplication.List(id);
        [HttpPost("search")]
        public async Task<ResponseDto<SearchResultDto<SearchOperadorDto>>> Search(SearchParamsDto<SearchOperadorFilterDto> searchParams)
            => await _OperadorApplication.Search(searchParams);

    }
}
