using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Operador;

namespace Reserva.Application.Abstractions.Dbo
{
    public interface IOperadorApplication
    {
        Task<ResponseDto<GetOperadorDto>> Create(CreateOperadorDto createDto);
        Task<ResponseDto<GetOperadorDto>> Update(UpdateOperadorDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetOperadorDto>> Get(int id);
        Task<ResponseDto<IEnumerable<ListOperadorDto>>> List(int id);
        Task<ResponseDto<SearchResultDto<SearchOperadorDto>>> Search(SearchParamsDto<SearchOperadorFilterDto> searchParams);

    }
}

