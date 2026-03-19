using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Calendario;
using Reserva.Dto.Dbo.HorarioCancha;
using System.Security.Claims;

namespace Reserva.Api.Controllers.Dbo
{
    [Route("api/[controller]")]
    [ApiController]
    //[Security.Authorize]
    public class CalendarioController : ControllerBase
    {
        private readonly ICalendarioApplication _calendarioApplication;

        public CalendarioController(ICalendarioApplication calendarioApplication)
        {
            _calendarioApplication = calendarioApplication;
        }

        [HttpGet("canchas-usuario")]
        public async Task<ResponseDto<List<CanchaUsuarioDto>>> GetCanchasUsuario()
        {
            var roles = User.FindAll("Roles").Select(c => c.Value).ToList();

            return await _calendarioApplication.GetCanchasUsuario(roles);
        }

        [HttpPost("disponibilidad-semanal")]
        public async Task<ResponseDto<DisponibilidadSemanalResponseDto>> GetDisponibilidadSemanal(DisponibilidadSemanalRequestDto request)
            => await _calendarioApplication.GetDisponibilidadSemanal(
                request.IdCancha,request.FechaInicio,request.FechaFin);

        [HttpPost("crear-reserva-operador")]
        public async Task<ResponseDto<ReservaOperadorResponseDto>> CrearReservaOperador(CrearReservaOperadorRequestDto request)
            => await _calendarioApplication.CrearReservaOperador(request);
        
        /// <summary>
        /// Busca clientes por nombre, apellido o teléfono
        /// </summary>
        [HttpGet("buscar-cliente")]
        public async Task<ResponseDto<List<ClienteDto>>> BuscarCliente([FromQuery] string termino)
        {
            return await _calendarioApplication.BuscarCliente(termino);
        }

        /// <summary>
        /// Obtiene las horas disponibles de una cancha en una fecha específica
        /// </summary>
        [HttpGet("horas-disponibles")]
        public async Task<ResponseDto<List<HorarioDisponibleDto>>> ObtenerHorasDisponibles(
            [FromQuery] int idCancha,
            [FromQuery] string fecha)
        {
            var fechaOffset = DateTimeOffset.Parse(fecha);
            return await _calendarioApplication.ObtenerHorasDisponibles(idCancha, fechaOffset);
        }

        /// <summary>
        /// Valida la disponibilidad de bloques de horarios
        /// </summary>
        [HttpPost("validar-disponibilidad")]
        public async Task<ResponseDto<ValidarDisponibilidadResponseDto>> ValidarDisponibilidad(
            [FromBody] ValidarDisponibilidadRequestDto request)
        {
            return await _calendarioApplication.ValidarDisponibilidad(request);
        }
    }
}
            