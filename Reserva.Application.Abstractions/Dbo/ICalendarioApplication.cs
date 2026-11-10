using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Calendario;
using Reserva.Dto.Dbo.HorarioCancha;

namespace Reserva.Application.Abstractions.Dbo
{
    public interface ICalendarioApplication
    {
        Task<ResponseDto<List<CanchaUsuarioDto>>> GetCanchasUsuario(List<string> roles);

        Task<ResponseDto<DisponibilidadSemanalResponseDto>> GetDisponibilidadSemanal(
            int idCancha,
            DateTime fechaInicio,
            DateTime fechaFin);

        Task<ResponseDto<ReservaOperadorResponseDto>> CrearReservaOperador(CrearReservaOperadorRequestDto request);

        Task<ResponseDto<ClientDto>> BuscarCliente(string dni, CancellationToken cancellationToken = default);

        Task<ResponseDto<List<HorarioDisponibleDto>>> ObtenerHorasDisponibles(int idCancha, DateTimeOffset fecha);

        Task<ResponseDto<ValidarDisponibilidadResponseDto>> ValidarDisponibilidad(
            ValidarDisponibilidadRequestDto request);
    }
}
