using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.HorarioCancha;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.HorarioCancha
{
    public class SelectComboHorarioCanchaQueryHandler : QueryHandlerBase<SelectComboHorarioCanchaQuery, IEnumerable<SelectComboHorarioCanchaDto>>
    {
        private readonly IRepository<Entity.HorarioCancha> _repository;

        public SelectComboHorarioCanchaQueryHandler(
            IMapper mapper,
            IRepository<Entity.HorarioCancha> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<SelectComboHorarioCanchaDto>>> HandleQuery(SelectComboHorarioCanchaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<SelectComboHorarioCanchaDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.Activo);
            var listDtos = _mapper?.Map<IEnumerable<SelectComboHorarioCanchaDto>>(list);

            response.UpdateData(listDtos ?? new List<SelectComboHorarioCanchaDto>());

            return await Task.FromResult(response);
        }
    }
}
