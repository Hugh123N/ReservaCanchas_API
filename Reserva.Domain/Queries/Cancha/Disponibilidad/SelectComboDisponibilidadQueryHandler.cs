using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Disponibilidad;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Disponibilidad
{
    public class SelectComboDisponibilidadQueryHandler : QueryHandlerBase<SelectComboDisponibilidadQuery, IEnumerable<SelectComboDisponibilidadDto>>
    {
        private readonly IRepository<Entity.Models.Disponibilidad> _repository;

        public SelectComboDisponibilidadQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.Disponibilidad> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<SelectComboDisponibilidadDto>>> HandleQuery(SelectComboDisponibilidadQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<SelectComboDisponibilidadDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.Activo);
            var listDtos = _mapper?.Map<IEnumerable<SelectComboDisponibilidadDto>>(list);

            response.UpdateData(listDtos ?? new List<SelectComboDisponibilidadDto>());

            return await Task.FromResult(response);
        }
    }
}
