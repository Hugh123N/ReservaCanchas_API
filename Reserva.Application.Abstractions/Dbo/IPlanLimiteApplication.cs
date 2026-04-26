using Reserva.Dto.Base;
using Reserva.Dto.Dbo.PlanLimite;

namespace Reserva.Application.Abstractions.Dbo
{
    public interface IPlanLimiteApplication
    {
        Task<ResponseDto<GetPlanLimiteDto>> Create(CreatePlanLimiteDto createDto);
        Task<ResponseDto<GetPlanLimiteDto>> Update(UpdatePlanLimiteDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetPlanLimiteDto>> Get(int id);
        Task<ResponseDto<IEnumerable<ListPlanLimiteDto>>> List(int id);

    }
}

