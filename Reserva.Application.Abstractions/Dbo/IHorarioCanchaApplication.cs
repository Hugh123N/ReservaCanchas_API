using Reserva.Dto.Base;
using Reserva.Dto.Dbo.HorarioCancha;

namespace Reserva.Application.Abstractions.Dbo
{
    public interface IHorarioCanchaApplication
    {
        Task<ResponseDto<GetHorarioCanchaDto>> Create(CreateHorarioCanchaDto createDto);
        Task<ResponseDto<GetHorarioCanchaDto>> Update(UpdateHorarioCanchaDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetHorarioCanchaDto>> Get(int id);
        Task<ResponseDto<IEnumerable<ListHorarioCanchaDto>>> List(int id);
        Task<ResponseDto<SearchResultDto<SearchHorarioCanchaDto>>> Search(SearchParamsDto<SearchHorarioCanchaFilterDto> searchParams);
        Task<ResponseDto<IEnumerable<SelectComboHorarioCanchaDto>>> SelectCombo();
        Task<ResponseDto<List<string>>> HorarioDisponible(DateTimeOffset fecha, int idCancha);
    }
}

