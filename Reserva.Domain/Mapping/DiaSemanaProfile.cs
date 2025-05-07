using AutoMapper;
using Reserva.Dto.Dbo.DiaSemana;
using Reserva.Dto.Dbo.DiaSemana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Domain.Mapping
{
    internal class DiaSemanaProfile : Profile
    {
        public DiaSemanaProfile()
        {
            CreateMap<Entity.Models.DiaSemana, DiaSemanaDto>().ReverseMap();
            CreateMap<Entity.Models.DiaSemana, CreateDiaSemanaDto>().ReverseMap();
            CreateMap<Entity.Models.DiaSemana, UpdateDiaSemanaDto>().ReverseMap();
            CreateMap<Entity.Models.DiaSemana, GetDiaSemanaDto>().ReverseMap();
            CreateMap<Entity.Models.DiaSemana, ListDiaSemanaDto>().ReverseMap();
            CreateMap<Entity.Models.DiaSemana, SelectComboDiaSemanaDto>().ReverseMap();
            CreateMap<Entity.Models.DiaSemana, SearchDiaSemanaDto>().ReverseMap();
        }
    }
}
