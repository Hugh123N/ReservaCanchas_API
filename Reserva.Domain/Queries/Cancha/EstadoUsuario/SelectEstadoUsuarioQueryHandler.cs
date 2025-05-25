using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Entity.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.EstadoUsuario;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Cancha.EstadoUsuario
{
    public class SelectEstadoUsuarioQueryHandler : SearchQueryHandlerBase<SelectEstadoUsuarioQuery, SelectEstadoUsuarioFilterDto, SelectEstadoUsuarioDto>
    {
        private readonly IRepository<Entity.Models.EstadoUsuario> _EstadoUsuarioRepository;

        public SelectEstadoUsuarioQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.EstadoUsuario> EstadoUsuarioRepository
        ) : base(mapper)
        {
            _EstadoUsuarioRepository = EstadoUsuarioRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SelectEstadoUsuarioDto>>> HandleQuery(SelectEstadoUsuarioQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SelectEstadoUsuarioDto>>();

            Expression<Func<Entity.Models.EstadoUsuario, bool>> filter = x => true;

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

            if (filters?.IdEstadoUsuario.HasValue == true)
                filter = filter.And(x => x.IdEstadoUsuario == filters.IdEstadoUsuario);

            var sorts = new List<SortExpression<Entity.Models.EstadoUsuario>>();

            if (request.SearchParams?.Sort != null)
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.Models.EstadoUsuario>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var EstadoUsuarios = await _EstadoUsuarioRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter
            );

            var EstadoUsuarioDtos = _mapper?.Map<IEnumerable<SelectEstadoUsuarioDto>>(EstadoUsuarios.Items);

            var searchResult = new SearchResultDto<SelectEstadoUsuarioDto>(
                EstadoUsuarioDtos ?? new List<SelectEstadoUsuarioDto>(),
                EstadoUsuarios.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
