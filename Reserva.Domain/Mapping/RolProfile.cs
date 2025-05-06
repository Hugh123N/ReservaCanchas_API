using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Reserva.Dto.Rol;

namespace Reserva.Domain.Mapping
{
    public class RolProfile : Profile
    {
        public RolProfile()
        {
            CreateMap<Entity.Models.Rol, SelectComboRolDto>().ReverseMap();
        }
    }
}
