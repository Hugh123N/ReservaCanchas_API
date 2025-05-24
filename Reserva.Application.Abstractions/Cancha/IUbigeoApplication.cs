using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Ubigeo;

namespace Reserva.Application.Abstractions.Cancha
{
    public interface IUbigeoApplication
    {
        Task<ResponseDto<GetUbigeoDto>> Create(CreateUbigeoDto createDto);
        Task<ResponseDto<GetUbigeoDto>> Update(UpdateUbigeoDto updateDto);
        Task<ResponseDto> Delete(string id);
        Task<ResponseDto<GetUbigeoDto>> Get(string id);
        Task<ResponseDto<IEnumerable<DepartamentoDto>>> List();
        Task<ResponseDto<SearchResultDto<SelectUbigeoDto>>> Select(SearchParamsDto<SelectUbigeoFilterDto> searchParams);

    }
}

