using System.Collections.Generic;

namespace Supernova.Dapper.Core.Extensions
{
    public static class LinqExtensions
    {
        public static HashSet<TEntity> ToHashSet<TEntity>(this IEnumerable<TEntity> source,
            IEqualityComparer<TEntity> comparer = null)
        {
            return new HashSet<TEntity>(source, comparer);
        }
    }
}
