using System.Collections.Generic;
using Supernova.Dapper.Core.Entities;

namespace Supernova.Dapper.Core.Repositories
{
    public interface IReadWriteDapperRepository<in TIdType, TEntity> :
        IReadOnlyDapperRepository<TIdType, TEntity>
        where TEntity : IEntity<TIdType>
    {
        void Insert(TEntity entity);

        void BulkInsert(IEnumerable<TEntity> entities);

        void Update(TEntity update);

        void BulkUpdate(IEnumerable<TEntity> entities);

        void Delete(TIdType id);

        void BulkDelete(IEnumerable<TIdType> ids);

        void BulkDelete(IEnumerable<TEntity> entities);
    }
}