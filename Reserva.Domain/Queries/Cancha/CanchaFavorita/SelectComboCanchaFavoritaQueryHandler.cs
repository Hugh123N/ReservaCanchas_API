using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.CanchaFavorita;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.CanchaFavorita
{
    public class SelectComboCanchaFavoritaQueryHandler : QueryHandlerBase<SelectComboCanchaFavoritaQuery, IEnumerable<SelectComboCanchaFavoritaDto>>
    {
        private readonly IRepository<Entity.Models.CanchaFavorita> _repository;

        public SelectComboCanchaFavoritaQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.CanchaFavorita> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<SelectComboCanchaFavoritaDto>>> HandleQuery(SelectComboCanchaFavoritaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<SelectComboCanchaFavoritaDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.Activo);
            var listDtos = _mapper?.Map<IEnumerable<SelectComboCanchaFavoritaDto>>(list);

            response.UpdateData(listDtos ?? new List<SelectComboCanchaFavoritaDto>());

            return await Task.FromResult(response);
        }
    }
}
