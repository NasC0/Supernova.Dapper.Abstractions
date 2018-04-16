using System.Collections.Generic;
using System.Data;
using Dapper;
using Supernova.Dapper.Core.Entities;
using Supernova.Dapper.Core.Factories;
using Supernova.Dapper.Core.Repositories;
using Supernova.Dapper.Parser.Core;
using Supernova.Dapper.Parser.Core.Models;

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
            ParsedQuery query = _queryParser.Insert(entity, false);
            using (IDbConnection sqlConnection = _connectionFactory.GetConnection())
            {
                sqlConnection.Query(query.Query.ToString(), query.Parameters);
            }
        }

        public abstract void BulkInsert(IEnumerable<TEntity> entities);

        public virtual void Update(TEntity update)
        {
            ParsedQuery query = _queryParser.Update(update);
            using (IDbConnection sqlConnection = _connectionFactory.GetConnection())
            {
                sqlConnection.Query(query.Query.ToString(), query.Parameters);
            }
        }

        public abstract void BulkUpdate(TEntity update);

        public abstract void Delete(TIdType id);

        public abstract void BulkDelete(IEnumerable<TIdType> ids);

        public abstract void BulkDelete(IEnumerable<TEntity> entities);
    }
}
