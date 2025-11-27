using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.CanchaFavorita;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.CanchaFavorita
{
    public class ListCanchaFavoritaQueryHandler : QueryHandlerBase<ListCanchaFavoritaQuery, IEnumerable<ListCanchaFavoritaDto>>
    {
        private readonly IRepository<Entity.CanchaFavorita> _repository;

        public ListCanchaFavoritaQueryHandler(
            IMapper mapper,
            IRepository<Entity.CanchaFavorita> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<ListCanchaFavoritaDto>>> HandleQuery(ListCanchaFavoritaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListCanchaFavoritaDto>>();

            if (!Guid.TryParse(request.IdUsuario, out var idUsuarioGuid))
            {
                response.AddErrorResult("El IdUsuario proporcionado no es un GUID válido.");
                return response;
            }

            var list = await _repository.FindByAsNoTrackingAsync(x => x.IdUsuario == idUsuarioGuid && x.Activo == true);
            var listDtos = _mapper?.Map<IEnumerable<ListCanchaFavoritaDto>>(list);

            response.UpdateData(listDtos ?? new List<ListCanchaFavoritaDto>());

            return await Task.FromResult(response);
        }
    }
}
