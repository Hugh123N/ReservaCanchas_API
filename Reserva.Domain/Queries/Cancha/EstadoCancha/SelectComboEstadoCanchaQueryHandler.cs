using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.EstadoCancha;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.EstadoCancha
{
    public class SelectComboEstadoCanchaQueryHandler : QueryHandlerBase<SelectComboEstadoCanchaQuery, IEnumerable<SelectComboEstadoCanchaDto>>
    {
        private readonly IRepository<Entity.Models.EstadoCancha> _repository;

        public SelectComboEstadoCanchaQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.EstadoCancha> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<SelectComboEstadoCanchaDto>>> HandleQuery(SelectComboEstadoCanchaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<SelectComboEstadoCanchaDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.Activo);
            var listDtos = _mapper?.Map<IEnumerable<SelectComboEstadoCanchaDto>>(list);

            response.UpdateData(listDtos ?? new List<SelectComboEstadoCanchaDto>());

            return await Task.FromResult(response);
        }
    }
}
