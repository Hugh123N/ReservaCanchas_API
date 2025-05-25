using Microsoft.EntityFrameworkCore;
//using Reserva.Audit.Common;
//using Reserva.Audit.RestClient;
using Reserva.Entity.Base;
using Reserva.Entity.Models;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using Reserva.Repository.Security;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace Reserva.Repository.Base
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext _dbContext;
        protected readonly IUserIdentity _userIdentity;
        //protected readonly IAuditService _auditService;

        public Repository(DbContext dbContext, IUserIdentity userIdentity/*, IAuditService auditService*/)
        {
            _dbContext = dbContext;
            _userIdentity = userIdentity;
            //_auditService = auditService;
        }

        public async Task<TEntity?> AddAsync(TEntity entity)
        {
            if (entity == null) return null;

            UpdateAuditTrails(entity);

            await _dbContext.Set<TEntity>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            //_auditService.AuditEntity(Operation.Create, _userIdentity.GetCurrentUser(), entity);

            return await Task.FromResult(entity);
        }

        public async Task<IEnumerable<TEntity>?> AddAsync(params TEntity[] entities)
        {
            if (entities == null) return null;
            if (!entities.Any()) return entities;

            foreach (var entity in entities) UpdateAuditTrails(entity);

            await _dbContext.Set<TEntity>().AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();

            //_auditService.AuditEntity(Operation.Create, _userIdentity.GetCurrentUser(), entities);

            return await Task.FromResult(entities);
        }

        public async Task<TEntity?> UpdateAsync(TEntity entity)
        {
            if (entity == null) return null;

            UpdateAuditTrails(entity, false);

            _dbContext.Set<TEntity>().Attach(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
            _dbContext.Update(entity);

            //_auditService.AuditEntity(Operation.Update, _userIdentity.GetCurrentUser(), entity);

            return await Task.FromResult(entity);
        }

        public async Task<IEnumerable<TEntity>?> UpdateAsync(params TEntity[] entities)
        {
            if (entities == null) return null;
            if (!entities.Any()) return entities;

            entities.ToList().ForEach(entity =>
            {
                UpdateAuditTrails(entity, false);

                _dbContext.Set<TEntity>().Attach(entity);
                _dbContext.Entry(entity).State = EntityState.Modified;
            });

            _dbContext.Set<TEntity>().UpdateRange(entities);

            //_auditService.AuditEntity(Operation.Update, _userIdentity.GetCurrentUser(), entities);

            return await Task.FromResult(entities);
        }

        public async Task<int> DeleteAsync(TEntity entity)
        {
            if (entity == null) return default;

            _dbContext.Set<TEntity>().Attach(entity);
            _dbContext.Set<TEntity>().Remove(entity);

            //_auditService.AuditEntity(Operation.Delete, _userIdentity.GetCurrentUser(), entity);

            return await Task.FromResult(default(int));
        }

        public async Task<int> DeleteAsync(params TEntity[] entities)
        {
            if (entities == null) return default;
            if (!entities.Any()) return default;

            entities.ToList().ForEach(entity => _dbContext.Set<TEntity>().Attach(entity));

            _dbContext.Set<TEntity>().RemoveRange(entities);

            //_auditService.AuditEntity(Operation.Delete, _userIdentity.GetCurrentUser(), entities);

            return await Task.FromResult(default(int));
        }

        public async Task<TEntity?> GetAsync(object keyValue)
            => await FindAll().FilterByKey(keyValue).FirstOrDefaultAsync();

        public async Task<TEntity?> GetAsNoTrackingAsync(object keyValue)
            => await FindAllAsNoTracking().FilterByKey(keyValue).FirstOrDefaultAsync();

        public async Task<TEntity?> GetAsync(params object[] keyValues)
            => await FindAll().FilterByKey(keyValues).FirstOrDefaultAsync();

        public async Task<TEntity?> GetAsNoTrackingAsync(params object[] keyValues)
            => await FindAllAsNoTracking().FilterByKey(keyValues).FirstOrDefaultAsync();

        public async Task<TEntity?> GetAsync(object keyValue, params Expression<Func<TEntity, object>>[] includeProperties)
            => await FindAll(includeProperties).FilterByKey(keyValue).FirstOrDefaultAsync();

        public async Task<TEntity?> GetAsNoTrackingAsync(object keyValue, params Expression<Func<TEntity, object>>[] includeProperties)
            => await FindAllAsNoTracking(includeProperties).FilterByKey(keyValue).FirstOrDefaultAsync();

        public async Task<TEntity?> GetAsync(object[] keyValues, params Expression<Func<TEntity, object>>[] includeProperties)
            => await FindAll(includeProperties).FilterByKey(keyValues).FirstOrDefaultAsync();

        public async Task<TEntity?> GetAsNoTrackingAsync(object[] keyValues, params Expression<Func<TEntity, object>>[] includeProperties)
            => await FindAllAsNoTracking(includeProperties).FilterByKey(keyValues).FirstOrDefaultAsync();

        public async Task<TEntity?> GetByAsync(Expression<Func<TEntity, bool>> filter)
            => await FindAll().Filter(filter).FirstOrDefaultAsync();

        public async Task<TEntity?> GetByAsNoTrackingAsync(Expression<Func<TEntity, bool>> filter)
            => await FindAllAsNoTracking().Filter(filter).FirstOrDefaultAsync();

        public async Task<TEntity?> GetByAsync(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includeProperties)
            => await FindAll(includeProperties).Filter(filter).FirstOrDefaultAsync();

        public async Task<TEntity?> GetByAsNoTrackingAsync(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includeProperties)
            => await FindAllAsNoTracking(includeProperties).Filter(filter).FirstOrDefaultAsync();

        public IQueryable<TEntity> FindAll()
            => _dbContext.Set<TEntity>();

        public IQueryable<TEntity> FindAllAsNoTracking()
            => _dbContext.Set<TEntity>().AsNoTracking();

        public IQueryable<TEntity> FindAll(params Expression<Func<TEntity, object>>[] includeProperties)
            => FindAll().IncludeProperties(includeProperties);

        public IQueryable<TEntity> FindAllAsNoTracking(params Expression<Func<TEntity, object>>[] includeProperties)
            => FindAllAsNoTracking().IncludeProperties(includeProperties);

        public async Task<IEnumerable<TEntity>> FindByAsync(Expression<Func<TEntity, bool>> filter)
            => await FindAll().Filter(filter).ToListAsync();

        public async Task<IEnumerable<TEntity>> FindByAsNoTrackingAsync(Expression<Func<TEntity, bool>> filter)
            => await FindAllAsNoTracking().Filter(filter).ToListAsync();

        public async Task<IEnumerable<TEntity>> FindByAsync(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includeProperties)
            => await FindAll(includeProperties).Filter(filter).ToListAsync();

        public async Task<IEnumerable<TEntity>> FindByAsNoTrackingAsync(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includeProperties)
            => await FindAllAsNoTracking(includeProperties).Filter(filter).ToListAsync();

        public async Task<SearchResult<TEntity>> SearchByAsync(int page, int pageSize, IEnumerable<SortExpression<TEntity>>? sortExpressions, Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includeProperties)
            => await SearchByAsync(false, page, pageSize, sortExpressions, filter, includeProperties);

        public async Task<SearchResult<TEntity>> SearchByAsNoTrackingAsync(int page, int pageSize, IEnumerable<SortExpression<TEntity>>? sortExpressions, Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includeProperties)
            => await SearchByAsync(true, page, pageSize, sortExpressions, filter, includeProperties);

        private async Task<SearchResult<TEntity>> SearchByAsync(bool asNoTracking, int page, int pageSize, IEnumerable<SortExpression<TEntity>>? sortExpressions, Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var findAll = asNoTracking ? FindAllAsNoTracking() : FindAll();

            if (filter != null) findAll = findAll.Filter(filter);

            var total = await findAll.CountAsync();

            if (sortExpressions != null && sortExpressions.Any())
            {
                var firstSortExpression = sortExpressions.First();

                if (firstSortExpression.Property != null)
                {
                    findAll = firstSortExpression.Direction == SortDirection.Asc ?
                        findAll.OrderBy(firstSortExpression.Property) :
                        findAll.OrderByDescending(firstSortExpression.Property);
                }

                if (sortExpressions.Count() > 1)
                {
                    foreach (var sortExpression in sortExpressions.Skip(1))
                    {
                        if (sortExpression?.Property == null) continue;
                        if (findAll is IOrderedQueryable<TEntity> sortExp && firstSortExpression.Property != null)
                        {
                            findAll = sortExpression.Direction == SortDirection.Asc ?
                                (sortExp).ThenBy(sortExpression.Property) :
                                (sortExp).ThenByDescending(sortExpression.Property);
                        }
                    }
                }
            }

            if (includeProperties != null && includeProperties.Any())
                findAll = findAll.IncludeProperties(includeProperties);

            var currentPage = page <= 0 ? 1 : page;
            var skip = ((currentPage - 1) * pageSize);

            findAll = findAll.Skip(skip).Take(pageSize);

            var items = await findAll.ToListAsync();

            return new SearchResult<TEntity>
            {
                Total = total,
                Items = items
            };
        }

        public async Task<int> SaveAsync()
            => await _dbContext.SaveChangesAsync();

        public void UpdateAuditTrails(TEntity entity, bool creation = true)
            => UpdateAuditTrailsDetails(entity, new List<Type>(), creation);

        private void UpdateAuditTrailsDetails<TDetail>(TDetail entity, ICollection<Type> baseTypes, bool creation = true)
        {
            if (entity == null) return;

            if (entity.GetType().IsSimple()) return;

            UpdateAuditInfo(entity, creation);

            foreach (var property in entity.GetType().GetProperties())
            {
                var propertyValue = property.GetValue(entity);

                if (propertyValue == null) continue;

                if (property.PropertyType.IsSimple()) continue;

                if (baseTypes.Contains(property.PropertyType)) continue;

                if (!baseTypes.Contains(entity.GetType())) baseTypes.Add(entity.GetType());

                if (property.PropertyType.IsCollection())
                {
                    if (propertyValue is IEnumerable entities)
                    {
                        var entitiesEnumerator = entities.GetEnumerator();
                        while (entitiesEnumerator.MoveNext())
                            UpdateAuditTrailsDetails(entitiesEnumerator.Current, baseTypes, creation);
                    }

                    continue;
                }

                UpdateAuditTrailsDetails(propertyValue, baseTypes, creation);
            }
        }

        private void UpdateAuditInfo(object entity, bool creation = true)
        {
            if (entity == null) return;

            var now = DateTimeOffset.UtcNow;
            var properties = entity.GetType().GetProperties();

            DateTimeOffset? nowNullable = now;
            var usuario = _userIdentity.GetCurrentUser();

            if (creation)
            {
                UpdateProperty(entity, properties, "UserNameCreate", usuario);
                UpdateProperty(entity, properties, "CreateDate", nowNullable, typeof(DateTimeOffset?));
                UpdateProperty(entity, properties, "CreateDate", now);
                UpdateProperty(entity, properties, "Activo", true);
            }

            UpdateProperty(entity, properties, "UserNameUpdate", usuario);
            UpdateProperty(entity, properties, "UpdateDate", nowNullable, typeof(DateTimeOffset?));
            UpdateProperty(entity, properties, "UpdateDate", now);
        }

        private void UpdateProperty(object entity, IEnumerable<PropertyInfo> properties, string propertyName, object? value, Type? type = null)
        {
            type ??= value?.GetType();

            var property = properties.FirstOrDefault(x => x.Name == propertyName);

            if (property == null) return;
            if (property.PropertyType.FullName != type?.FullName) return;

            property.SetValue(entity, value);
        }
    }
}
