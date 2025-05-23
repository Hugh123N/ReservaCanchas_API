using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Zona;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Zona
{
    public class SelectComboZonaQueryHandler : QueryHandlerBase<SelectComboZonaQuery, IEnumerable<SelectComboZonaDto>>
    {
        private readonly IRepository<Entity.Models.Zona> _repository;

        public SelectComboZonaQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.Zona> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<SelectComboZonaDto>>> HandleQuery(SelectComboZonaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<SelectComboZonaDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.Activo);
            var listDtos = _mapper?.Map<IEnumerable<SelectComboZonaDto>>(list);

            response.UpdateData(listDtos ?? new List<SelectComboZonaDto>());

            return await Task.FromResult(response);
        }
    }
}
