using Reserva.Dto.Base;
using Reserva.Dto.Dbo.ImagenCancha;

namespace Reserva.Application.Abstractions.Dbo
{
    public interface IImagenCanchaApplication
    {
        Task<ResponseDto<GetImagenCanchaDto>> Create(CreateImagenCanchaDto createDto);
        Task<ResponseDto<GetImagenCanchaDto>> Update(UpdateImagenCanchaDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetImagenCanchaDto>> Get(int id);
        Task<ResponseDto<IEnumerable<ListImagenCanchaDto>>> List(int id);
        Task<ResponseDto<SearchResultDto<SearchImagenCanchaDto>>> Search(SearchParamsDto<SearchImagenCanchaFilterDto> searchParams);
        Task<ResponseDto<IEnumerable<SelectComboImagenCanchaDto>>> SelectCombo();
        Task<ResponseDto<SearchResultDto<SelectImagenCanchaDto>>> Select(SearchParamsDto<SelectImagenCanchaFilterDto> searchParams);

    }
}

