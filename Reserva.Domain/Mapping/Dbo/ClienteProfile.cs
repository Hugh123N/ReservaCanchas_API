using AutoMapper;
using Reserva.Dto.Dbo.Cliente;

namespace Reserva.Domain.Mapping.Cliente
{
    public class ClienteProfile : Profile
    {
        public ClienteProfile()
        {
            CreateMap<Entity.Cliente, ClienteDto>()
                .ReverseMap();

            CreateMap<Entity.Cliente, CreateClienteDto>()
                .ReverseMap();

            CreateMap<Entity.Cliente, UpdateClienteDto>()
                .ReverseMap();

            CreateMap<Entity.Cliente, GetClienteDto>()
                .ReverseMap();

            CreateMap<Entity.Cliente, ListClienteDto>()
                .ReverseMap();

            CreateMap<Entity.Cliente, SearchClienteDto>()
                .ReverseMap();
        }
    }
}
