using Reserva.Dto.Base;
using Reserva.Dto.Cancha.IntentoLogin;

namespace Reserva.Application.Abstractions.Cancha
{
    public interface IIntentoLoginApplication
    {
        Task<ResponseDto<GetIntentoLoginDto>> Create(CreateIntentoLoginDto createDto);
        Task<ResponseDto<GetIntentoLoginDto>> Update(UpdateIntentoLoginDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetIntentoLoginDto>> Get(int id);
        Task<ResponseDto<IEnumerable<ListIntentoLoginDto>>> List(int id);
        Task<ResponseDto<SearchResultDto<SearchIntentoLoginDto>>> Search(SearchParamsDto<SearchIntentoLoginFilterDto> searchParams);
        Task<ResponseDto<SearchResultDto<SelectIntentoLoginDto>>> Select(SearchParamsDto<SelectIntentoLoginFilterDto> searchParams);

    }
}

