using System.Collections.Generic;
using Supernova.Dapper.Core.Entities;

namespace Supernova.Dapper.Core.Repositories
{
    public interface IReadWriteDapperRepository<in TIdType, TEntity> :
        IReadOnlyDapperRepository<TIdType, TEntity>
        where TEntity : IEntity<TIdType>
    {
        void Insert(TEntity entity);

        void Insert(TEntity entity, params string[] ignoreColumns);

        void BulkInsert(IEnumerable<TEntity> entities);

        void BulkInsert(IEnumerable<TEntity> entities, params string[] ignoreColumns);

        void Update(TEntity update);

        void Update(TEntity update, params string[] ignoreColumns);

        void BulkUpdate(IEnumerable<TEntity> entities);

        void BulkUpdate(IEnumerable<TEntity> entities, params string[] ignoreColumns);

        void Delete(TIdType id);

        void BulkDelete(IEnumerable<TIdType> ids);

        void BulkDelete(IEnumerable<TEntity> entities);
    }
}