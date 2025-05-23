using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Departamento;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Departamento
{
    public class GetDepartamentoQueryHandler : QueryHandlerBase<GetDepartamentoQuery, GetDepartamentoDto>
    {
        private readonly IRepository<Entity.Models.Departamento> _DepartamentoRepository;

        public GetDepartamentoQueryHandler(
            IMapper mapper,
            GetDepartamentoQueryValidator validator,
            IRepository<Entity.Models.Departamento> DepartamentoRepository
        ) : base(mapper, validator)
        {
            _DepartamentoRepository = DepartamentoRepository;
        }

        protected override async Task<ResponseDto<GetDepartamentoDto>> HandleQuery(GetDepartamentoQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetDepartamentoDto>();
            var Departamento = await _DepartamentoRepository.GetByAsync(x => x.IdDepartamento == request.Id);
            var DepartamentoDto = _mapper?.Map<GetDepartamentoDto>(Departamento);

            if (Departamento != null && DepartamentoDto != null)
            {
                response.UpdateData(DepartamentoDto);
            }

            return await Task.FromResult(response);
        }
    }
}
