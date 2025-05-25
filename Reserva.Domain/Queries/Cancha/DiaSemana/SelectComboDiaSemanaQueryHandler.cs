using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.DiaSemana;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.DiaSemana
{
    public class SelectComboDiaSemanaQueryHandler : QueryHandlerBase<SelectComboDiaSemanaQuery, IEnumerable<SelectComboDiaSemanaDto>>
    {
        private readonly IRepository<Entity.Models.DiaSemana> _repository;

        public SelectComboDiaSemanaQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.DiaSemana> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<SelectComboDiaSemanaDto>>> HandleQuery(SelectComboDiaSemanaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<SelectComboDiaSemanaDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.Activo);
            var listDtos = _mapper?.Map<IEnumerable<SelectComboDiaSemanaDto>>(list);

            response.UpdateData(listDtos ?? new List<SelectComboDiaSemanaDto>());

            return await Task.FromResult(response);
        }
    }
}
