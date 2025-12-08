using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Hora;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.Hora
{
    public class SelectComboHoraQueryHandler : QueryHandlerBase<SelectComboHoraQuery, IEnumerable<SelectComboHoraDto>>
    {
        private readonly IRepository<Entity.Hora> _repository;

        public SelectComboHoraQueryHandler(
            IMapper mapper,
            IRepository<Entity.Hora> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<SelectComboHoraDto>>> HandleQuery(SelectComboHoraQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<SelectComboHoraDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.Activo);
            var listDtos = _mapper?.Map<IEnumerable<SelectComboHoraDto>>(list);

            response.UpdateData(listDtos ?? new List<SelectComboHoraDto>());

            return await Task.FromResult(response);
        }
    }
}
