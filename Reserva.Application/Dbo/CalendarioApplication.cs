using MediatR;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Dbo.Calendario;
using Reserva.Domain.Queries.Dbo.Calendario;
using Reserva.Domain.Queries.Dbo.HorarioCancha;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Calendario;
using Reserva.Dto.Dbo.HorarioCancha;

namespace Reserva.Application.Dbo
{
    public class CalendarioApplication : ApplicationBase, ICalendarioApplication
    {
        public CalendarioApplication(IMediator mediator) : base(mediator)
        {
        }

        public async Task<ResponseDto<List<CanchaUsuarioDto>>> GetCanchasUsuario(List<string> roles)
        {
            var query = new GetCanchasUsuarioQuery
            {
                Roles = roles
            };

            return await _mediator.Send(query);
        }

        public async Task<ResponseDto<DisponibilidadSemanalResponseDto>> GetDisponibilidadSemanal(
            int idCancha,
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            var query = new GetDisponibilidadSemanalQuery
            {
                IdCancha = idCancha,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };

            return await _mediator.Send(query);
        }

        // Métodos para crear reserva desde operador

        public async Task<ResponseDto<ReservaOperadorResponseDto>> CrearReservaOperador(CrearReservaOperadorRequestDto request)
        {
            var command = new CrearReservaOperadorCommand
            {
                RequestDto = request
            };

            return await _mediator.Send(command);
        }

        public async Task<ResponseDto<List<ClienteDto>>> BuscarCliente(string terminoBusqueda)
        {
            var query = new BuscarClienteQuery
            {
                TerminoBusqueda = terminoBusqueda
            };

            return await _mediator.Send(query);
        }

        public async Task<ResponseDto<List<HorarioDisponibleDto>>> ObtenerHorasDisponibles(int idCancha, DateTimeOffset fecha)
        {
            var query = new GetCanchaByFechaQuery(fecha, idCancha);

            return await _mediator.Send(query);
        }

        public async Task<ResponseDto<ValidarDisponibilidadResponseDto>> ValidarDisponibilidad(
            ValidarDisponibilidadRequestDto request)
        {
            var query = new ValidarDisponibilidadQuery
            {
                IdCancha = request.IdCancha,
                Horarios = request.Horarios
            };

            return await _mediator.Send(query);
        }
    }
}
