using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Cancha.Departamento;
using Reserva.Domain.Queries.Cancha.Departamento;
using Reserva.Dto.Cancha.Departamento;

namespace Reserva.Application.Cancha
{
    public class DepartamentoApplication : ApplicationBase, IDepartamentoApplication
    {
        public DepartamentoApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetDepartamentoDto>> Create(CreateDepartamentoDto createDto)
            => await _mediator.Send(new CreateDepartamentoCommand(createDto));
        public async Task<ResponseDto<GetDepartamentoDto>> Update(UpdateDepartamentoDto updateDto)
            => await _mediator.Send(new UpdateDepartamentoCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeleteDepartamentoCommand(id));
        public async Task<ResponseDto<GetDepartamentoDto>> Get(int id)
            => await _mediator.Send(new GetDepartamentoQuery(id));
        public async Task<ResponseDto<IEnumerable<ListDepartamentoDto>>> List(int id)
            => await _mediator.Send(new ListDepartamentoQuery(id));
        public async Task<ResponseDto<SearchResultDto<SearchDepartamentoDto>>> Search(SearchParamsDto<SearchDepartamentoFilterDto> searchParams)
            => await _mediator.Send(new SearchDepartamentoQuery(searchParams));
        public async Task<ResponseDto<IEnumerable<SelectComboDepartamentoDto>>> SelectCombo()
            => await _mediator.Send(new SelectComboDepartamentoQuery());
        public async Task<ResponseDto<SearchResultDto<SelectDepartamentoDto>>> Select(SearchParamsDto<SelectDepartamentoFilterDto> searchParams)
             => await _mediator.Send(new SelectDepartamentoQuery(searchParams));

    }
}
