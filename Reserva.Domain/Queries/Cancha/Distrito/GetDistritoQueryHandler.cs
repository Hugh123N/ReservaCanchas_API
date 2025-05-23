using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Distrito;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Distrito
{
    public class GetDistritoQueryHandler : QueryHandlerBase<GetDistritoQuery, GetDistritoDto>
    {
        private readonly IRepository<Entity.Models.Distrito> _DistritoRepository;

        public GetDistritoQueryHandler(
            IMapper mapper,
            GetDistritoQueryValidator validator,
            IRepository<Entity.Models.Distrito> DistritoRepository
        ) : base(mapper, validator)
        {
            _DistritoRepository = DistritoRepository;
        }

        protected override async Task<ResponseDto<GetDistritoDto>> HandleQuery(GetDistritoQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetDistritoDto>();
            var Distrito = await _DistritoRepository.GetByAsync(x => x.IdDistrito == request.Id);
            var DistritoDto = _mapper?.Map<GetDistritoDto>(Distrito);

            if (Distrito != null && DistritoDto != null)
            {
                response.UpdateData(DistritoDto);
            }

            return await Task.FromResult(response);
        }
    }
}
