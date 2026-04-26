using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Plane;

namespace Reserva.Application.Abstractions.Dbo
{
    public interface IPlaneApplication
    {
        Task<ResponseDto<GetPlaneDto>> Create(CreatePlaneDto createDto);
        Task<ResponseDto<GetPlaneDto>> Update(UpdatePlaneDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetPlaneDto>> Get(int id);
        Task<ResponseDto<IEnumerable<ListPlaneDto>>> List(int id);

    }
}

