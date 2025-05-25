using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Ubigeo;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Ubigeo
{
    public class GetUbigeoQueryHandler : QueryHandlerBase<GetUbigeoQuery, GetUbigeoDto>
    {
        private readonly IRepository<Entity.Models.Ubigeo> _UbigeoRepository;

        public GetUbigeoQueryHandler(
            IMapper mapper,
            GetUbigeoQueryValidator validator,
            IRepository<Entity.Models.Ubigeo> UbigeoRepository
        ) : base(mapper, validator)
        {
            _UbigeoRepository = UbigeoRepository;
        }

        protected override async Task<ResponseDto<GetUbigeoDto>> HandleQuery(GetUbigeoQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetUbigeoDto>();
            var Ubigeo = await _UbigeoRepository.GetByAsync(x => x.CodigoUbigeo == request.Id);
            var UbigeoDto = _mapper?.Map<GetUbigeoDto>(Ubigeo);

            if (Ubigeo != null && UbigeoDto != null)
            {
                response.UpdateData(UbigeoDto);
            }

            return await Task.FromResult(response);
        }
    }
}
