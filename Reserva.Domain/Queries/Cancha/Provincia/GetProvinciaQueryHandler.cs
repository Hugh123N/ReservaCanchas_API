using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Provincia;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Provincia
{
    public class GetProvinciaQueryHandler : QueryHandlerBase<GetProvinciaQuery, GetProvinciaDto>
    {
        private readonly IRepository<Entity.Models.Provincia> _ProvinciaRepository;

        public GetProvinciaQueryHandler(
            IMapper mapper,
            GetProvinciaQueryValidator validator,
            IRepository<Entity.Models.Provincia> ProvinciaRepository
        ) : base(mapper, validator)
        {
            _ProvinciaRepository = ProvinciaRepository;
        }

        protected override async Task<ResponseDto<GetProvinciaDto>> HandleQuery(GetProvinciaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetProvinciaDto>();
            var Provincia = await _ProvinciaRepository.GetByAsync(x => x.IdProvincia == request.Id);
            var ProvinciaDto = _mapper?.Map<GetProvinciaDto>(Provincia);

            if (Provincia != null && ProvinciaDto != null)
            {
                response.UpdateData(ProvinciaDto);
            }

            return await Task.FromResult(response);
        }
    }
}
