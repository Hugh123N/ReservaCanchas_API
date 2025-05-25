using AutoMapper;
using Reserva.Dto.Cancha.Notificacion;

namespace Reserva.Domain.Mapping.Notificacion
{
    public class NotificacionProfile : Profile
    {
        public NotificacionProfile()
        {
            CreateMap<Entity.Models.Notificacion, NotificacionDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Notificacion, CreateNotificacionDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Notificacion, UpdateNotificacionDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Notificacion, GetNotificacionDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Notificacion, ListNotificacionDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Notificacion, SelectComboNotificacionDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Notificacion, SearchNotificacionDto>()
                .ReverseMap();
        }
    }
}
