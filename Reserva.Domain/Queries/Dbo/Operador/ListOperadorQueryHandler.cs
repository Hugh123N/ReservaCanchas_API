using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Operador;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.Operador
{
    public class ListOperadorQueryHandler : QueryHandlerBase<ListOperadorQuery, IEnumerable<ListOperadorDto>>
    {
        private readonly IRepository<Entity.Operador> _repository;

        public ListOperadorQueryHandler(
            IMapper mapper,
            IRepository<Entity.Operador> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<ListOperadorDto>>> HandleQuery(ListOperadorQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListOperadorDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.IdOperador == request.Id);
            var listDtos = _mapper?.Map<IEnumerable<ListOperadorDto>>(list);

            response.UpdateData(listDtos ?? new List<ListOperadorDto>());

            return await Task.FromResult(response);
        }
    }
}
