using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Comision;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Comision
{
    public class SelectComboComisionQueryHandler : QueryHandlerBase<SelectComboComisionQuery, IEnumerable<SelectComboComisionDto>>
    {
        private readonly IRepository<Entity.Models.Comision> _repository;

        public SelectComboComisionQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.Comision> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<SelectComboComisionDto>>> HandleQuery(SelectComboComisionQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<SelectComboComisionDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.Activo);
            var listDtos = _mapper?.Map<IEnumerable<SelectComboComisionDto>>(list);

            response.UpdateData(listDtos ?? new List<SelectComboComisionDto>());

            return await Task.FromResult(response);
        }
    }
}
