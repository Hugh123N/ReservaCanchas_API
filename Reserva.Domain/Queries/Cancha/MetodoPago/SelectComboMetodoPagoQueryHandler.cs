using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.MetodoPago;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.MetodoPago
{
    public class SelectComboMetodoPagoQueryHandler : QueryHandlerBase<SelectComboMetodoPagoQuery, IEnumerable<SelectComboMetodoPagoDto>>
    {
        private readonly IRepository<Entity.Models.MetodoPago> _repository;

        public SelectComboMetodoPagoQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.MetodoPago> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<SelectComboMetodoPagoDto>>> HandleQuery(SelectComboMetodoPagoQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<SelectComboMetodoPagoDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.Activo);
            var listDtos = _mapper?.Map<IEnumerable<SelectComboMetodoPagoDto>>(list);

            response.UpdateData(listDtos ?? new List<SelectComboMetodoPagoDto>());

            return await Task.FromResult(response);
        }
    }
}
