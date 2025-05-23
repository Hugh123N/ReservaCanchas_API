using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Cancha.Disponibilidad;
using Reserva.Domain.Queries.Cancha.Disponibilidad;
using Reserva.Dto.Cancha.Disponibilidad;

namespace Reserva.Application.Cancha
{
    public class DisponibilidadApplication : ApplicationBase, IDisponibilidadApplication
    {
        public DisponibilidadApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetDisponibilidadDto>> Create(CreateDisponibilidadDto createDto)
            => await _mediator.Send(new CreateDisponibilidadCommand(createDto));
        public async Task<ResponseDto<GetDisponibilidadDto>> Update(UpdateDisponibilidadDto updateDto)
            => await _mediator.Send(new UpdateDisponibilidadCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeleteDisponibilidadCommand(id));
        public async Task<ResponseDto<GetDisponibilidadDto>> Get(int id)
            => await _mediator.Send(new GetDisponibilidadQuery(id));
        public async Task<ResponseDto<IEnumerable<ListDisponibilidadDto>>> List(int id)
            => await _mediator.Send(new ListDisponibilidadQuery(id));
        public async Task<ResponseDto<SearchResultDto<SearchDisponibilidadDto>>> Search(SearchParamsDto<SearchDisponibilidadFilterDto> searchParams)
            => await _mediator.Send(new SearchDisponibilidadQuery(searchParams));
        public async Task<ResponseDto<IEnumerable<SelectComboDisponibilidadDto>>> SelectCombo()
            => await _mediator.Send(new SelectComboDisponibilidadQuery());
        public async Task<ResponseDto<SearchResultDto<SelectDisponibilidadDto>>> Select(SearchParamsDto<SelectDisponibilidadFilterDto> searchParams)
             => await _mediator.Send(new SelectDisponibilidadQuery(searchParams));

    }
}
