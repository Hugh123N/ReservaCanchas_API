using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Departamento;

namespace Reserva.Application.Abstractions.Cancha
{
    public interface IDepartamentoApplication
    {
        Task<ResponseDto<GetDepartamentoDto>> Create(CreateDepartamentoDto createDto);
        Task<ResponseDto<GetDepartamentoDto>> Update(UpdateDepartamentoDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetDepartamentoDto>> Get(int id);
        Task<ResponseDto<IEnumerable<ListDepartamentoDto>>> List(int id);
        Task<ResponseDto<SearchResultDto<SearchDepartamentoDto>>> Search(SearchParamsDto<SearchDepartamentoFilterDto> searchParams);
        Task<ResponseDto<IEnumerable<SelectComboDepartamentoDto>>> SelectCombo();
        Task<ResponseDto<SearchResultDto<SelectDepartamentoDto>>> Select(SearchParamsDto<SelectDepartamentoFilterDto> searchParams);

    }
}

