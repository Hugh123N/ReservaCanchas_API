using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.EstadoPago;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.EstadoPago
{
    public class SelectComboEstadoPagoQueryHandler : QueryHandlerBase<SelectComboEstadoPagoQuery, IEnumerable<SelectComboEstadoPagoDto>>
    {
        private readonly IRepository<Entity.Models.EstadoPago> _repository;

        public SelectComboEstadoPagoQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.EstadoPago> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<SelectComboEstadoPagoDto>>> HandleQuery(SelectComboEstadoPagoQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<SelectComboEstadoPagoDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.Activo);
            var listDtos = _mapper?.Map<IEnumerable<SelectComboEstadoPagoDto>>(list);

            response.UpdateData(listDtos ?? new List<SelectComboEstadoPagoDto>());

            return await Task.FromResult(response);
        }
    }
}
