using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Cancha;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Cancha
{
    public class GetCanchaQueryHandler : QueryHandlerBase<GetCanchaQuery, GetCanchaDto>
    {
        private readonly IRepository<Entity.Models.Cancha> _CanchaRepository;

        public GetCanchaQueryHandler(
            IMapper mapper,
            GetCanchaQueryValidator validator,
            IRepository<Entity.Models.Cancha> CanchaRepository
        ) : base(mapper, validator)
        {
            _CanchaRepository = CanchaRepository;
        }

        protected override async Task<ResponseDto<GetCanchaDto>> HandleQuery(GetCanchaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetCanchaDto>();
            var Cancha = await _CanchaRepository.GetByAsync(x => x.IdCancha == request.Id);
            var CanchaDto = _mapper?.Map<GetCanchaDto>(Cancha);

            if (Cancha != null && CanchaDto != null)
            {
                response.UpdateData(CanchaDto);
            }

            return await Task.FromResult(response);
        }
    }
}
