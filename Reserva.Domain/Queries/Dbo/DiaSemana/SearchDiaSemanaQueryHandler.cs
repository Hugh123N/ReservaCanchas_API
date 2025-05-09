using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Entity.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.DiaSemana;
using System.Linq.Expressions;
using Reserva.Entity.Models;
using Microsoft.EntityFrameworkCore;
using Reserva.Entity.Extensions;
using Reserva.Repository.Extensions;

namespace Reserva.Domain.Queries.Dbo.DiaSemana
{
    public class SearchDiaSemanaQueryHandler : SearchQueryHandlerBase<SearchDiaSemanaQuery, SearchDiaSemanaFilterDto, SearchDiaSemanaDto>
    {
        private readonly ReservaCanchasContext _context;

        public SearchDiaSemanaQueryHandler(
            IMapper mapper,
            ReservaCanchasContext context
        ) : base(mapper)
        {
            _context = context;
        }

        protected override async Task<ResponseDto<SearchResultDto<SearchDiaSemanaDto>>> HandleQuery(SearchDiaSemanaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SearchDiaSemanaDto>>();

            Expression<Func<Entity.Models.DiaSemana, bool>> filter = x => true;

            var filters = request.SearchParams?.Filter;

            if (!string.IsNullOrEmpty(filters!.Nombre)) {
                filter = filter.And(x => x.Nombre.Contains(filters.Nombre));
            }
            

            var sorts = new List<SortExpression<Entity.Models.DiaSemana>>();

            if (request.SearchParams?.Sort != null)
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.Models.DiaSemana>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var DiaSeamanas = await SearchByAsync(
                true,
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter
            );

            var DiaSemanaDtos = _mapper?.Map<IEnumerable<SearchDiaSemanaDto>>(DiaSeamanas.Items);

            var searchResult = new SearchResultDto<SearchDiaSemanaDto>(
                DiaSemanaDtos ?? new List<SearchDiaSemanaDto>(),
                DiaSeamanas.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }

        private async Task<SearchResult<TEntity>> SearchByAsync<TEntity>(bool asNoTracking, int page, int pageSize,
            IEnumerable<SortExpression<TEntity>>? sortExpressions, Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includeProperties) where TEntity : class
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (asNoTracking)
                query = query.AsNoTracking();

            if (includeProperties != null && includeProperties.Any())
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            if (filter != null)
                query = query.Where(filter);

            var total = await query.CountAsync();

            if (sortExpressions != null && sortExpressions.Any())
            {
                var orderedQuery = sortExpressions.First().Direction == SortDirection.Asc
                    ? query.OrderBy(sortExpressions.First().Property)
                    : query.OrderByDescending(sortExpressions.First().Property);

                foreach (var sortExpression in sortExpressions.Skip(1))
                {
                    if (sortExpression?.Property == null) continue;

                    orderedQuery = sortExpression.Direction == SortDirection.Asc
                        ? ((IOrderedQueryable<TEntity>)orderedQuery).ThenBy(sortExpression.Property)
                        : ((IOrderedQueryable<TEntity>)orderedQuery).ThenByDescending(sortExpression.Property);
                }

                query = orderedQuery;
            }

            var currentPage = page <= 0 ? 1 : page;
            var skip = (currentPage - 1) * pageSize;

            var items = await query.Skip(skip).Take(pageSize).ToListAsync();

            return new SearchResult<TEntity>
            {
                Total = total,
                Items = items
            };
        }
    }
}
