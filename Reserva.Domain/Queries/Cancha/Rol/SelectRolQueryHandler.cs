using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Entity.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Rol;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Cancha.Rol
{
    public class SelectRolQueryHandler : SearchQueryHandlerBase<SelectRolQuery, SelectRolFilterDto, SelectRolDto>
    {
        private readonly IRepository<Entity.Models.Rol> _RolRepository;

        public SelectRolQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.Rol> RolRepository
        ) : base(mapper)
        {
            _RolRepository = RolRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SelectRolDto>>> HandleQuery(SelectRolQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SelectRolDto>>();

            Expression<Func<Entity.Models.Rol, bool>> filter = x => true;

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

            if (filters?.IdRol.HasValue == true)
                filter = filter.And(x => x.IdRol == filters.IdRol);

            var sorts = new List<SortExpression<Entity.Models.Rol>>();

            if (request.SearchParams?.Sort != null)
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.Models.Rol>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var Rols = await _RolRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter
            );

            var RolDtos = _mapper?.Map<IEnumerable<SelectRolDto>>(Rols.Items);

            var searchResult = new SearchResultDto<SelectRolDto>(
                RolDtos ?? new List<SelectRolDto>(),
                Rols.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
