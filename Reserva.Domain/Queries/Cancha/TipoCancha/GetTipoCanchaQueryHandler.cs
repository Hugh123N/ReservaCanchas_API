using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.TipoCancha;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.TipoCancha
{
    public class GetTipoCanchaQueryHandler : QueryHandlerBase<GetTipoCanchaQuery, GetTipoCanchaDto>
    {
        private readonly IRepository<Entity.Models.TipoCancha> _TipoCanchaRepository;

        public GetTipoCanchaQueryHandler(
            IMapper mapper,
            GetTipoCanchaQueryValidator validator,
            IRepository<Entity.Models.TipoCancha> TipoCanchaRepository
        ) : base(mapper, validator)
        {
            _TipoCanchaRepository = TipoCanchaRepository;
        }

        protected override async Task<ResponseDto<GetTipoCanchaDto>> HandleQuery(GetTipoCanchaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetTipoCanchaDto>();
            var TipoCancha = await _TipoCanchaRepository.GetByAsync(x => x.IdTipoCancha == request.Id);
            var TipoCanchaDto = _mapper?.Map<GetTipoCanchaDto>(TipoCancha);

            if (TipoCancha != null && TipoCanchaDto != null)
            {
                response.UpdateData(TipoCanchaDto);
            }

            return await Task.FromResult(response);
        }
    }
}
