using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Departamento;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Departamento
{
    public class SelectComboDepartamentoQueryHandler : QueryHandlerBase<SelectComboDepartamentoQuery, IEnumerable<SelectComboDepartamentoDto>>
    {
        private readonly IRepository<Entity.Models.Departamento> _repository;

        public SelectComboDepartamentoQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.Departamento> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<SelectComboDepartamentoDto>>> HandleQuery(SelectComboDepartamentoQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<SelectComboDepartamentoDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.Activo);
            var listDtos = _mapper?.Map<IEnumerable<SelectComboDepartamentoDto>>(list);

            response.UpdateData(listDtos ?? new List<SelectComboDepartamentoDto>());

            return await Task.FromResult(response);
        }
    }
}
