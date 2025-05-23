using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Departamento;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Departamento
{
    public class ListDepartamentoQueryHandler : QueryHandlerBase<ListDepartamentoQuery, IEnumerable<ListDepartamentoDto>>
    {
        private readonly IRepository<Entity.Models.Departamento> _repository;

        public ListDepartamentoQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.Departamento> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<ListDepartamentoDto>>> HandleQuery(ListDepartamentoQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListDepartamentoDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.IdDepartamento == request.Id);
            var listDtos = _mapper?.Map<IEnumerable<ListDepartamentoDto>>(list);

            response.UpdateData(listDtos ?? new List<ListDepartamentoDto>());

            return await Task.FromResult(response);
        }
    }
}
