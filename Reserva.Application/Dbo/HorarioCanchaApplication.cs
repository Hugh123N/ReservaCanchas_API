using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Dbo.HorarioCancha;
using Reserva.Domain.Queries.Dbo.HorarioCancha;
using Reserva.Dto.Dbo.HorarioCancha;

namespace Reserva.Application.Dbo
{
    public class HorarioCanchaApplication : ApplicationBase, IHorarioCanchaApplication
    {
        public HorarioCanchaApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetHorarioCanchaDto>> Create(CreateHorarioCanchaDto createDto)
            => await _mediator.Send(new CreateHorarioCanchaCommand(createDto));
        public async Task<ResponseDto<GetHorarioCanchaDto>> Update(UpdateHorarioCanchaDto updateDto)
            => await _mediator.Send(new UpdateHorarioCanchaCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeleteHorarioCanchaCommand(id));
        public async Task<ResponseDto<GetHorarioCanchaDto>> Get(int id)
            => await _mediator.Send(new GetHorarioCanchaQuery(id));
        public async Task<ResponseDto<IEnumerable<ListHorarioCanchaDto>>> List(int id)
            => await _mediator.Send(new ListHorarioCanchaQuery(id));
        public async Task<ResponseDto<SearchResultDto<SearchHorarioCanchaDto>>> Search(SearchParamsDto<SearchHorarioCanchaFilterDto> searchParams)
            => await _mediator.Send(new SearchHorarioCanchaQuery(searchParams));
        public async Task<ResponseDto<IEnumerable<SelectComboHorarioCanchaDto>>> SelectCombo()
            => await _mediator.Send(new SelectComboHorarioCanchaQuery());
        public async Task<ResponseDto<List<string>>> HorarioDisponible(DateTimeOffset fecha, int idCancha)
            => await _mediator.Send(new GetCanchaByFechaQuery(fecha, idCancha));
    }
}
