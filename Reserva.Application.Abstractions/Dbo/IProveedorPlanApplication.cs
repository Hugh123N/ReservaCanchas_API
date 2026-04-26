using Reserva.Dto.Base;
using Reserva.Dto.Dbo.ProveedorPlan;

namespace Reserva.Application.Abstractions.Dbo
{
    public interface IProveedorPlanApplication
    {
        Task<ResponseDto<GetProveedorPlanDto>> Create(CreateProveedorPlanDto createDto);
        Task<ResponseDto<GetProveedorPlanDto>> Update(UpdateProveedorPlanDto updateDto);
        Task<ResponseDto> Delete(int id);
        Task<ResponseDto<GetProveedorPlanDto>> Get(int id);
        Task<ResponseDto<IEnumerable<ListProveedorPlanDto>>> List(int id);
        Task<ResponseDto<SearchResultDto<SearchProveedorPlanDto>>> Search(SearchParamsDto<SearchProveedorPlanFilterDto> searchParams);

    }
}

