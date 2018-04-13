using System.Collections.Generic;
using Supernova.Dapper.Core.Entities;

namespace Supernova.Dapper.Core.Repositories
{
    public interface IReadWriteDapperRepository<in TIdType, in TInputEntity, out TResultEntity> : 
        IReadOnlyDapperRepository<TIdType, TResultEntity> where TInputEntity : IEntity<TIdType>
    {
        void Insert(TInputEntity entity);

        void BulkInsert(IEnumerable<TInputEntity> entities);

        void Update(TInputEntity update);

        void BulkUpdate(TInputEntity update);

        void Delete(TIdType id);

        void BulkDelete(IEnumerable<TIdType> ids);

        void BulkDelete(IEnumerable<TInputEntity> entities);
    }
}