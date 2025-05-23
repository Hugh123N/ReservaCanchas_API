using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.DetallePago;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.DetallePago
{
    public class SelectComboDetallePagoQueryHandler : QueryHandlerBase<SelectComboDetallePagoQuery, IEnumerable<SelectComboDetallePagoDto>>
    {
        private readonly IRepository<Entity.Models.DetallePago> _repository;

        public SelectComboDetallePagoQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.DetallePago> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<SelectComboDetallePagoDto>>> HandleQuery(SelectComboDetallePagoQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<SelectComboDetallePagoDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => (bool)x.IdPagoNavigation!.Activo!);
            var listDtos = _mapper?.Map<IEnumerable<SelectComboDetallePagoDto>>(list);

            response.UpdateData(listDtos ?? new List<SelectComboDetallePagoDto>());

            return await Task.FromResult(response);
        }
    }
}
