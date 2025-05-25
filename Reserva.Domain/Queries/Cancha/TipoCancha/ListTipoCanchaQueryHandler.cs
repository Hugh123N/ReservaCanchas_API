using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.TipoCancha;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.TipoCancha
{
    public class ListTipoCanchaQueryHandler : QueryHandlerBase<ListTipoCanchaQuery, IEnumerable<ListTipoCanchaDto>>
    {
        private readonly IRepository<Entity.Models.TipoCancha> _repository;

        public ListTipoCanchaQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.TipoCancha> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<ListTipoCanchaDto>>> HandleQuery(ListTipoCanchaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListTipoCanchaDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.IdTipoCancha == request.Id);
            var listDtos = _mapper?.Map<IEnumerable<ListTipoCanchaDto>>(list);

            response.UpdateData(listDtos ?? new List<ListTipoCanchaDto>());

            return await Task.FromResult(response);
        }
    }
}
