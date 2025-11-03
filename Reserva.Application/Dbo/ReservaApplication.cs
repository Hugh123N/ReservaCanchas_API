using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Dbo.Reserva;
using Reserva.Domain.Queries.Dbo.Reserva;
using Reserva.Dto.Dbo.Reserva;

namespace Reserva.Application.Dbo
{
    public class ReservaApplication : ApplicationBase, IReservaApplication
    {
        public ReservaApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<ReservaConPagoDto>> Create(CreateReservaDto createDto)
            => await _mediator.Send(new CreateReservaCommand(createDto));
        public async Task<ResponseDto<GetReservaDto>> Update(UpdateReservaDto updateDto)
            => await _mediator.Send(new UpdateReservaCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeleteReservaCommand(id));
        public async Task<ResponseDto<GetReservaDto>> Get(int id)
            => await _mediator.Send(new GetReservaQuery(id));
        public async Task<ResponseDto<IEnumerable<ListReservaDto>>> List(int id)
            => await _mediator.Send(new ListReservaQuery(id));
        public async Task<ResponseDto<SearchResultDto<SearchReservaDto>>> Search(SearchParamsDto<SearchReservaFilterDto> searchParams)
            => await _mediator.Send(new SearchReservaQuery(searchParams));
        public async Task<ResponseDto<IEnumerable<SelectComboReservaDto>>> SelectCombo()
            => await _mediator.Send(new SelectComboReservaQuery());
        public async Task<ResponseDto<SearchResultDto<SelectReservaDto>>> Select(SearchParamsDto<SelectReservaFilterDto> searchParams)
             => await _mediator.Send(new SelectReservaQuery(searchParams));

        // Operaciones para el Operador
        public async Task<ResponseDto<GetReservaDto>> ConfirmarReservaOperador(ConfirmarReservaOperadorDto confirmarDto)
            => await _mediator.Send(new ConfirmarReservaOperadorCommand(confirmarDto));

        public async Task<ResponseDto<GetReservaDto>> LiberarReservaOperador(LiberarReservaOperadorDto liberarDto)
            => await _mediator.Send(new LiberarReservaOperadorCommand(liberarDto));

        public async Task<ResponseDto<IEnumerable<ReservaPendienteOperadorDto>>> ObtenerReservasPendientesOperador(int idProveedor)
            => await _mediator.Send(new ReservasPendientesOperadorQuery(idProveedor));

        // Operaciones para el Cliente
        public async Task<ResponseDto<IEnumerable<ReservaClienteDto>>> ObtenerReservasCliente(Guid idUsuario)
            => await _mediator.Send(new ReservasClienteQuery(idUsuario));
    }
}
