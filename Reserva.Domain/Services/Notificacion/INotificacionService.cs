using Reserva.Entity;

namespace Reserva.Domain.Services.Notificacion
{
    /// <summary>
    /// Servicio de notificaciones para enviar mensajes por Email y WhatsApp
    /// </summary>
    public interface INotificacionService
    {
        /// <summary>
        /// Notifica al operador sobre una nueva reserva pendiente
        /// </summary>
        Task NotificarNuevaReservaPendienteAsync(Entity.Reserva reserva, Cancha cancha, AspNetUsers cliente, List<Operador> operadores, string horariosFormateado);

        /// <summary>
        /// Notifica al cliente que su reserva fue confirmada
        /// </summary>
        Task NotificarReservaConfirmadaAsync(Entity.Reserva reserva, Cancha cancha, AspNetUsers cliente, Entity.Pago pago, string horariosFormateado);

        /// <summary>
        /// Notifica al operador sobre reservas próximas a expirar
        /// </summary>
        Task NotificarReservaProximaExpirarAsync(Entity.Reserva reserva, Cancha cancha, AspNetUsers cliente, List<Operador> operadores);

        /// <summary>
        /// Notifica al operador que una reserva expiró
        /// </summary>
        Task NotificarReservaExpiradaAsync(Entity.Reserva reserva, Cancha cancha, List<Operador> operadores);

        /// <summary>
        /// Notifica al cliente que su reserva fue cancelada
        /// </summary>
        Task NotificarReservaCanceladaAsync(Entity.Reserva reserva, AspNetUsers cliente, string motivo);

        /// <summary>
        /// Envia recordatorio al cliente 1 hora antes de su reserva confirmada
        /// </summary>
        Task NotificarRecordatorioReservaAsync(Entity.Reserva reserva, Cancha cancha, AspNetUsers cliente, string horariosFormateado);
    }
}
