using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Cancha.Provincia;
using Reserva.Domain.Queries.Cancha.Provincia;
using Reserva.Dto.Cancha.Provincia;

namespace Reserva.Application.Cancha
{
    public class ProvinciaApplication : ApplicationBase, IProvinciaApplication
    {
        public ProvinciaApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetProvinciaDto>> Create(CreateProvinciaDto createDto)
            => await _mediator.Send(new CreateProvinciaCommand(createDto));
        public async Task<ResponseDto<GetProvinciaDto>> Update(UpdateProvinciaDto updateDto)
            => await _mediator.Send(new UpdateProvinciaCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeleteProvinciaCommand(id));
        public async Task<ResponseDto<GetProvinciaDto>> Get(int id)
            => await _mediator.Send(new GetProvinciaQuery(id));
        public async Task<ResponseDto<IEnumerable<ListProvinciaDto>>> List(int id)
            => await _mediator.Send(new ListProvinciaQuery(id));
        public async Task<ResponseDto<SearchResultDto<SearchProvinciaDto>>> Search(SearchParamsDto<SearchProvinciaFilterDto> searchParams)
            => await _mediator.Send(new SearchProvinciaQuery(searchParams));
        public async Task<ResponseDto<IEnumerable<SelectComboProvinciaDto>>> SelectCombo()
            => await _mediator.Send(new SelectComboProvinciaQuery());
        public async Task<ResponseDto<SearchResultDto<SelectProvinciaDto>>> Select(SearchParamsDto<SelectProvinciaFilterDto> searchParams)
             => await _mediator.Send(new SelectProvinciaQuery(searchParams));

    }
}
