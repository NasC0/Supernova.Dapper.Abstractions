using System;
using System.Collections.Generic;
using Supernova.Dapper.Core.Entities;
using Supernova.Dapper.Core.Factories;
using Supernova.Dapper.Core.Repositories;

namespace Supernova.Dapper.Base
{
    public abstract class ReadWriteDapperRepository<TIdType, TInputEntity, TResultEntity> : ReadOnlyDapperRepository<TIdType, TResultEntity>,
        IReadWriteDapperRepository<TIdType, TInputEntity, TResultEntity>
        where TInputEntity : IEntity<TIdType>
    {
        protected ReadWriteDapperRepository(IConnectionFactory connectionFactory)
            : base(connectionFactory)
        {
        }

        public virtual void Insert(TInputEntity entity)
        {

        }

        public abstract void BulkInsert(IEnumerable<TInputEntity> entities);

        public abstract void Update(TInputEntity update);

        public abstract void BulkUpdate(TInputEntity update);

        public abstract void Delete(TIdType id);

        public abstract void BulkDelete(IEnumerable<TIdType> ids);

        public abstract void BulkDelete(IEnumerable<TInputEntity> entities);
    }
}
