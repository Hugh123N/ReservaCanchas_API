using MediatR;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Dbo.DiaSemana;
using Reserva.Domain.Queries.Dbo.DiaSemana;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.DiaSemana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Application.Dbo
{
    public class DiaSemanaApplication : ApplicationBase, IDiaSemanaApplication
    {
        public DiaSemanaApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetDiaSemanaDto>> Create(CreateDiaSemanaDto createDto)
            => await _mediator.Send(new CreateDiaSemanaCommand(createDto));
        public async Task<ResponseDto<GetDiaSemanaDto>> Update(UpdateDiaSemanaDto updateDto)
            => await _mediator.Send(new UpdateDiaSemanaCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeleteDiaSemanaCommand(id));
        public async Task<ResponseDto<GetDiaSemanaDto>> Get(int id)
            => await _mediator.Send(new GetDiaSemanaQuery(id));
        public async Task<ResponseDto<IEnumerable<ListDiaSemanaDto>>> List(int id)
            => await _mediator.Send(new ListDiaSemanaQuery(id));
        public async Task<ResponseDto<SearchResultDto<SearchDiaSemanaDto>>> Search(SearchParamsDto<SearchDiaSemanaFilterDto> searchParams)
            => await _mediator.Send(new SearchDiaSemanaQuery(searchParams));
        public async Task<ResponseDto<IEnumerable<SelectComboDiaSemanaDto>>> SelectCombo()
            => await _mediator.Send(new SelectComboDiaSemanaQuery());
        public async Task<ResponseDto<SearchResultDto<SelectDiaSemanaDto>>> Select(SearchParamsDto<SelectDiaSemanaFilterDto> searchParams)
             => await _mediator.Send(new SelectDiaSemanaQuery(searchParams));
    }
}
