using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.EstadoReserva;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.EstadoReserva
{
    public class SelectComboEstadoReservaQueryHandler : QueryHandlerBase<SelectComboEstadoReservaQuery, IEnumerable<SelectComboEstadoReservaDto>>
    {
        private readonly IRepository<Entity.Models.EstadoReserva> _repository;

        public SelectComboEstadoReservaQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.EstadoReserva> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<SelectComboEstadoReservaDto>>> HandleQuery(SelectComboEstadoReservaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<SelectComboEstadoReservaDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.Activo);
            var listDtos = _mapper?.Map<IEnumerable<SelectComboEstadoReservaDto>>(list);

            response.UpdateData(listDtos ?? new List<SelectComboEstadoReservaDto>());

            return await Task.FromResult(response);
        }
    }
}
