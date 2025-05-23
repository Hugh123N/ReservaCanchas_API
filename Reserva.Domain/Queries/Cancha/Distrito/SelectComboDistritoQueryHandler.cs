using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Distrito;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Distrito
{
    public class SelectComboDistritoQueryHandler : QueryHandlerBase<SelectComboDistritoQuery, IEnumerable<SelectComboDistritoDto>>
    {
        private readonly IRepository<Entity.Models.Distrito> _repository;

        public SelectComboDistritoQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.Distrito> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<SelectComboDistritoDto>>> HandleQuery(SelectComboDistritoQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<SelectComboDistritoDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.Activo);
            var listDtos = _mapper?.Map<IEnumerable<SelectComboDistritoDto>>(list);

            response.UpdateData(listDtos ?? new List<SelectComboDistritoDto>());

            return await Task.FromResult(response);
        }
    }
}
