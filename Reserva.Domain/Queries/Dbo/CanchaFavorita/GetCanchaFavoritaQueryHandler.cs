using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.CanchaFavorita;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.CanchaFavorita
{
    public class GetCanchaFavoritaQueryHandler : QueryHandlerBase<GetCanchaFavoritaQuery, GetCanchaFavoritaDto>
    {
        private readonly IRepository<Entity.CanchaFavorita> _CanchaFavoritaRepository;

        public GetCanchaFavoritaQueryHandler(
            IMapper mapper,
            GetCanchaFavoritaQueryValidator validator,
            IRepository<Entity.CanchaFavorita> CanchaFavoritaRepository
        ) : base(mapper, validator)
        {
            _CanchaFavoritaRepository = CanchaFavoritaRepository;
        }

        protected override async Task<ResponseDto<GetCanchaFavoritaDto>> HandleQuery(GetCanchaFavoritaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetCanchaFavoritaDto>();
            var CanchaFavorita = await _CanchaFavoritaRepository.GetByAsync(x => x.IdCancha == request.Id);
            var CanchaFavoritaDto = _mapper?.Map<GetCanchaFavoritaDto>(CanchaFavorita);

            if (CanchaFavorita != null && CanchaFavoritaDto != null)
            {
                response.UpdateData(CanchaFavoritaDto);
            }

            return await Task.FromResult(response);
        }
    }
}
