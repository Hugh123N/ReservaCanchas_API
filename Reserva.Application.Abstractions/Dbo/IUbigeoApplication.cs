using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Ubigeo;

namespace Reserva.Application.Abstractions.Dbo
{
    public interface IUbigeoApplication
    {
        Task<ResponseDto<GetUbigeoDto>> Create(CreateUbigeoDto createDto);
        Task<ResponseDto<GetUbigeoDto>> Update(UpdateUbigeoDto updateDto);
        Task<ResponseDto> Delete(string id);
        Task<ResponseDto<GetUbigeoDto>> Get(string id);
        Task<ResponseDto<IEnumerable<DepartamentoDto>>> List();
        Task<ResponseDto<SearchResultDto<SelectUbigeoDto>>> Select(SearchParamsDto<SelectUbigeoFilterDto> searchParams);
        Task<ResponseDto<IEnumerable<GetUbigeoDto>>> ListAll();
    }
}

