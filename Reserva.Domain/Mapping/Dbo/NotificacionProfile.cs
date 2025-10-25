using AutoMapper;
using Reserva.Dto.Dbo.Notificacion;

namespace Reserva.Domain.Mapping.Notificacion
{
    public class NotificacionProfile : Profile
    {
        public NotificacionProfile()
        {
            CreateMap<Entity.Notificacion, NotificacionDto>()
                .ReverseMap();

            CreateMap<Entity.Notificacion, CreateNotificacionDto>()
                .ReverseMap();

            CreateMap<Entity.Notificacion, UpdateNotificacionDto>()
                .ReverseMap();

            CreateMap<Entity.Notificacion, GetNotificacionDto>()
                .ReverseMap();

            CreateMap<Entity.Notificacion, ListNotificacionDto>()
                .ReverseMap();

            CreateMap<Entity.Notificacion, SelectComboNotificacionDto>()
                .ReverseMap();

            CreateMap<Entity.Notificacion, SearchNotificacionDto>()
                .ReverseMap();
        }
    }
}
