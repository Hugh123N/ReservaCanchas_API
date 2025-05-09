using Reserva.Dto.Base;
using Reserva.Dto.Dbo.DiaSemana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Application.Abstractions.Dbo
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
