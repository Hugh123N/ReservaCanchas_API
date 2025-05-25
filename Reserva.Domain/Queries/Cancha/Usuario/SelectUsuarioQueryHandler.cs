using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Entity.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Usuario;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Cancha.Usuario
{
    public class SelectUsuarioQueryHandler : SearchQueryHandlerBase<SelectUsuarioQuery, SelectUsuarioFilterDto, SelectUsuarioDto>
    {
        private readonly IRepository<Entity.Models.Usuario> _UsuarioRepository;

        public SelectUsuarioQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.Usuario> UsuarioRepository
        ) : base(mapper)
        {
            _UsuarioRepository = UsuarioRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SelectUsuarioDto>>> HandleQuery(SelectUsuarioQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SelectUsuarioDto>>();

            Expression<Func<Entity.Models.Usuario, bool>> filter = x => true;

            var filters = request.SearchParams?.Filter;

            /*
            if (filters?.FechaDesde.HasValue == true || filters?.FechaHasta.HasValue == true)
            {
                if (filters?.FechaDesde.HasValue == true)
                {
                    var fechaDesde = filters.FechaDesde.GetStartDate();
                    filter = filter.And(x => x.Fecha >= fechaDesde);
                }

                if (filters?.FechaHasta.HasValue == true)
                {
                    var fechaHasta = filters.FechaHasta.GetEndDate();
                    filter = filter.And(x => x.Fecha < fechaHasta);
                }
            }
            */

            if (filters?.IdUsuario.HasValue == true)
                filter = filter.And(x => x.IdUsuario == filters.IdUsuario);

            var sorts = new List<SortExpression<Entity.Models.Usuario>>();

            if (request.SearchParams?.Sort != null)
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.Models.Usuario>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var Usuarios = await _UsuarioRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter
            );

            var UsuarioDtos = _mapper?.Map<IEnumerable<SelectUsuarioDto>>(Usuarios.Items);

            var searchResult = new SearchResultDto<SelectUsuarioDto>(
                UsuarioDtos ?? new List<SelectUsuarioDto>(),
                Usuarios.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
