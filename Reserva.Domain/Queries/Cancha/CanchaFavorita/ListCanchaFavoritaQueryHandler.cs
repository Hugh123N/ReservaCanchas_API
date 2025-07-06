using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.CanchaFavorita;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.CanchaFavorita
{
    public class ListCanchaFavoritaQueryHandler : QueryHandlerBase<ListCanchaFavoritaQuery, IEnumerable<ListCanchaFavoritaDto>>
    {
        private readonly IRepository<Entity.Models.CanchaFavorita> _repository;

        public ListCanchaFavoritaQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.CanchaFavorita> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<ListCanchaFavoritaDto>>> HandleQuery(ListCanchaFavoritaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListCanchaFavoritaDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.IdCancha == request.Id);
            var listDtos = _mapper?.Map<IEnumerable<ListCanchaFavoritaDto>>(list);

            response.UpdateData(listDtos ?? new List<ListCanchaFavoritaDto>());

            return await Task.FromResult(response);
        }
    }
}
