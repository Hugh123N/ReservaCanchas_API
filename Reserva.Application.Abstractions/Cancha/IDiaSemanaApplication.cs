using Reserva.Dto.Base;
using Reserva.Dto.Cancha.DiaSemana;

namespace Reserva.Application.Abstractions.Cancha
{
    public interface IDiaSemanaApplication
    {
        Task<ResponseDto<GetDiaSemanaDto>> Create(CreateDiaSemanaDto createDto);
        Task<ResponseDto<GetDiaSemanaDto>> Update(UpdateDiaSemanaDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetDiaSemanaDto>> Get(int id);
        Task<ResponseDto<IEnumerable<ListDiaSemanaDto>>> List(int id);
        Task<ResponseDto<SearchResultDto<SearchDiaSemanaDto>>> Search(SearchParamsDto<SearchDiaSemanaFilterDto> searchParams);
        Task<ResponseDto<IEnumerable<SelectComboDiaSemanaDto>>> SelectCombo();
        Task<ResponseDto<SearchResultDto<SelectDiaSemanaDto>>> Select(SearchParamsDto<SelectDiaSemanaFilterDto> searchParams);

    }
}

