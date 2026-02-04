using Reserva.Dto.Dbo.HorarioCancha;

namespace Reserva.Domain.Services
{
    /// <summary>
    /// Servicio de dominio para gestión de horarios de canchas.
    /// Proporciona lógica de negocio para expansión y compresión de horarios.
    /// </summary>
    public class HorarioCanchaService
    {
        private const int BLOQUES_POR_HORA = 2;
        private const int MAX_ID_HORA = 48; // 48 bloques de 30 min por día (00:00 - 23:30)

        public List<CreateHorarioCanchaDto> ExpandirHorariosCreate(List<CreateHorarioCanchaDto> horariosUsuario)
        {
            if (horariosUsuario == null || !horariosUsuario.Any())
                return new List<CreateHorarioCanchaDto>();

            var horariosExpandidos = new List<CreateHorarioCanchaDto>();

            foreach (var horario in horariosUsuario)
            {
                // Validar que el IdHoraInicio no exceda el máximo (último bloque 23:30)
                if (horario.IdHoraInicio > MAX_ID_HORA - 1) 
                {
                    throw new ArgumentException(
                        $"IdHoraInicio {horario.IdHoraInicio} excede el máximo permitido. " +
                        $"El último horario válido es IdHora {MAX_ID_HORA - 1} (23:00)."
                    );
                }

                // Dividir el precio entre 2 bloques
                decimal precioPorBloque = horario.PrecioHora / BLOQUES_POR_HORA;

                // Calcular IdHoraFin con lógica circular (después de 48 viene 1)
                int idHoraFinBloque1 = horario.IdHoraInicio + 1;
                int idHoraInicioBloque2 = horario.IdHoraInicio + 1;
                int idHoraFinBloque2 = horario.IdHoraInicio + 2;

                // Si excede 48, volver a 1 (00:00)
                if (idHoraFinBloque2 > MAX_ID_HORA)
                {
                    idHoraFinBloque2 = 1;
                }

                // Bloque 1: Hora en punto (ej: 06:00 - 06:30)
                horariosExpandidos.Add(new CreateHorarioCanchaDto
                {
                    IdCancha = horario.IdCancha,
                    IdDiaSemana = horario.IdDiaSemana,
                    IdHoraInicio = horario.IdHoraInicio,
                    IdHoraFin = idHoraFinBloque1,
                    PrecioHora = precioPorBloque
                });

                // Bloque 2: Media hora (ej: 06:30 - 07:00)
                horariosExpandidos.Add(new CreateHorarioCanchaDto
                {
                    IdCancha = horario.IdCancha,
                    IdDiaSemana = horario.IdDiaSemana,
                    IdHoraInicio = idHoraInicioBloque2,
                    IdHoraFin = idHoraFinBloque2,
                    PrecioHora = precioPorBloque
                });
            }

            return horariosExpandidos;
        }

        public List<UpdateHorarioCanchaDto> ExpandirHorariosUpdate(
            List<UpdateHorarioCanchaDto> horariosUsuario,
            ICollection<Entity.HorarioCancha> horariosExistentes)
        {
            if (horariosUsuario == null || !horariosUsuario.Any())
                return new List<UpdateHorarioCanchaDto>();

            var horariosExpandidos = new List<UpdateHorarioCanchaDto>();

            foreach (var horario in horariosUsuario)
            {
                // Validar que el IdHoraInicio no exceda el máximo
                if (horario.IdHoraInicio > MAX_ID_HORA - 1)
                {
                    throw new ArgumentException(
                        $"IdHoraInicio {horario.IdHoraInicio} excede el máximo permitido. " +
                        $"El último horario válido es IdHora {MAX_ID_HORA - 1} (23:00)."
                    );
                }

                decimal precioPorBloque = horario.PrecioHora / BLOQUES_POR_HORA;

                // Calcular IdHoraFin con lógica circular (después de 48 viene 1)
                int idHoraFinBloque1 = horario.IdHoraInicio + 1;
                int idHoraInicioBloque2 = horario.IdHoraInicio + 1;
                int idHoraFinBloque2 = horario.IdHoraInicio + 2;

                // Si excede 48, volver a 1 (00:00)
                if (idHoraFinBloque2 > MAX_ID_HORA)
                {
                    idHoraFinBloque2 = 1;
                }

                // Bloque 1: Hora en punto - usa el ID que viene del frontend
                horariosExpandidos.Add(new UpdateHorarioCanchaDto
                {
                    IdHorarioCancha = horario.IdHorarioCancha,  // ID del primer bloque
                    IdCancha = horario.IdCancha,
                    IdDiaSemana = horario.IdDiaSemana,
                    IdHoraInicio = horario.IdHoraInicio,
                    IdHoraFin = idHoraFinBloque1,
                    PrecioHora = precioPorBloque
                });

                // Bloque 2: Media hora - buscar en horarios existentes por combinación única
                var segundoBloque = horariosExistentes.FirstOrDefault(h =>
                    h.IdCancha == horario.IdCancha &&
                    h.IdDiaSemana == horario.IdDiaSemana &&
                    h.IdHoraInicio == idHoraInicioBloque2
                );

                horariosExpandidos.Add(new UpdateHorarioCanchaDto
                {
                    IdHorarioCancha = segundoBloque?.IdHorarioCancha ?? 0,  // ID encontrado o 0 si es nuevo
                    IdCancha = horario.IdCancha,
                    IdDiaSemana = horario.IdDiaSemana,
                    IdHoraInicio = idHoraInicioBloque2,
                    IdHoraFin = idHoraFinBloque2,
                    PrecioHora = precioPorBloque
                });
            }

            return horariosExpandidos;
        }

        public List<Entity.HorarioCancha> ComprimirHorarios(List<Entity.HorarioCancha> horariosExpandidos)
        {
            if (horariosExpandidos == null || !horariosExpandidos.Any())
                return new List<Entity.HorarioCancha>();

            var horariosComprimidos = new List<Entity.HorarioCancha>();

            var grupos = horariosExpandidos
                .GroupBy(h => new
                {
                    h.IdDiaSemana,
                    // Calcular la hora en punto: si es impar usa ese ID, si es par resta 1
                    HoraEnPunto = h.IdHoraInicio % 2 == 0 ? h.IdHoraInicio - 1 : h.IdHoraInicio
                })
                .ToList();

            foreach (var grupo in grupos)
            {
                // Ordenar bloques por IdHoraInicio y tomar el primero
                var bloques = grupo.OrderBy(h => h.IdHoraInicio).ToList();
                var primerBloque = bloques.First();

                // Sumar los precios de todos los bloques del grupo (normalmente serán 2)
                var precioTotal = grupo.Sum(h => h.PrecioHora);

                horariosComprimidos.Add(new Entity.HorarioCancha
                {
                    IdHorarioCancha = primerBloque.IdHorarioCancha,  
                    IdCancha = primerBloque.IdCancha,
                    IdDiaSemana = primerBloque.IdDiaSemana,
                    IdHoraInicio = grupo.Key.HoraEnPunto,             
                    IdHoraFin = null,                                  
                    PrecioHora = precioTotal,                         
                    IdDiaSemanaNavigation = primerBloque.IdDiaSemanaNavigation,
                    IdHoraInicioNavigation = primerBloque.IdHoraInicioNavigation
                });
            }

            return horariosComprimidos.OrderBy(h => h.IdDiaSemana).ThenBy(h => h.IdHoraInicio).ToList();
        }
    }
}
