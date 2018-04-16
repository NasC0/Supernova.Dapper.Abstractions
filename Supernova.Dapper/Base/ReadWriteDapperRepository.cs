using System.Collections.Generic;
using Supernova.Dapper.Core.Entities;
using Supernova.Dapper.Core.Factories;
using Supernova.Dapper.Core.Repositories;
using Supernova.Dapper.Parser.Core;

namespace Supernova.Dapper.Base
{
    public abstract class ReadWriteDapperRepository<TIdType, TEntity> : ReadOnlyDapperRepository<TIdType, TEntity>,
        IReadWriteDapperRepository<TIdType, TEntity>
        where TEntity : IEntity<TIdType>
    {
        protected ReadWriteDapperRepository(IConnectionFactory connectionFactory,
            IParser<TIdType> queryParser)
            : base(connectionFactory, queryParser)
        {
        }

        public virtual void Insert(TEntity entity)
        {

        }

        public abstract void BulkInsert(IEnumerable<TEntity> entities);

        public abstract void Update(TEntity update);

        public abstract void BulkUpdate(TEntity update);

        public abstract void Delete(TIdType id);

        public abstract void BulkDelete(IEnumerable<TIdType> ids);

        public abstract void BulkDelete(IEnumerable<TEntity> entities);
    }
}
