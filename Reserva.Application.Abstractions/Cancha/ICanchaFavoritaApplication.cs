using Reserva.Dto.Base;
using Reserva.Dto.Cancha.CanchaFavorita;

namespace Reserva.Application.Abstractions.Cancha
{
    public interface ICanchaFavoritaApplication
    {
        Task<ResponseDto<GetCanchaFavoritaDto>> Create(CreateCanchaFavoritaDto createDto);
        Task<ResponseDto<GetCanchaFavoritaDto>> Update(UpdateCanchaFavoritaDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetCanchaFavoritaDto>> Get(int id);
        Task<ResponseDto<IEnumerable<ListCanchaFavoritaDto>>> List(int id);
        Task<ResponseDto<SearchResultDto<SearchCanchaFavoritaDto>>> Search(SearchParamsDto<SearchCanchaFavoritaFilterDto> searchParams);
        Task<ResponseDto<IEnumerable<SelectComboCanchaFavoritaDto>>> SelectCombo();
        Task<ResponseDto<SearchResultDto<SelectCanchaFavoritaDto>>> Select(SearchParamsDto<SelectCanchaFavoritaFilterDto> searchParams);

    }
}

