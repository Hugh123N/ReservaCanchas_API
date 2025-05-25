using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Pago;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Pago
{
    public class SelectComboPagoQueryHandler : QueryHandlerBase<SelectComboPagoQuery, IEnumerable<SelectComboPagoDto>>
    {
        private readonly IRepository<Entity.Models.Pago> _repository;

        public SelectComboPagoQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.Pago> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<SelectComboPagoDto>>> HandleQuery(SelectComboPagoQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<SelectComboPagoDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.Activo);
            var listDtos = _mapper?.Map<IEnumerable<SelectComboPagoDto>>(list);

            response.UpdateData(listDtos ?? new List<SelectComboPagoDto>());

            return await Task.FromResult(response);
        }
    }
}
