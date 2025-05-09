using Reserva.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Entity.Extensions
{
    public static class SortExtensions
    {
        public static SortExpression<TEntity> GetSortExpression<TEntity>(string? direction, string? property)
        {
            if (string.IsNullOrEmpty(property)) return null!;

            var expression = GetSortExpression<TEntity>(property);
            if (expression == null) return null!;

            return new SortExpression<TEntity>
            {
                Direction = direction?.ToLower() == "asc" ? SortDirection.Asc : SortDirection.Desc,
                Property = expression
            };
        }

        private static Expression<Func<TEntity, object>> GetSortExpression<TEntity>(string property)
        {
            var prop = typeof(TEntity).GetProperties().FirstOrDefault(x => x.Name.Equals(property, StringComparison.OrdinalIgnoreCase));
            if (prop == null) return null!;

            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var propertyAccess = Expression.Property(parameter, prop);
            var converted = Expression.Convert(propertyAccess, typeof(object));

            return Expression.Lambda<Func<TEntity, object>>(converted, parameter);
        }
    }
}
