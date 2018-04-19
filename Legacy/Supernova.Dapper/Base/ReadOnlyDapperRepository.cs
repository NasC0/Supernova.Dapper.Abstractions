using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Supernova.Dapper.Core.Entities;
using Supernova.Dapper.Core.Enums;
using Supernova.Dapper.Core.Factories;
using Supernova.Dapper.Core.Repositories;
using Supernova.Dapper.Parser.Core;
using Supernova.Dapper.Parser.Core.Models;

namespace Supernova.Dapper.Base
{
    public abstract class ReadOnlyDapperRepository<TIdType, TEntity> : 
        IReadOnlyDapperRepository<TIdType, TEntity> 
        where TEntity : IEntity<TIdType>
    {
        protected readonly IParser<TIdType> _queryParser;
        protected readonly IConnectionFactory _connectionFactory;

        protected ReadOnlyDapperRepository(IConnectionFactory connectionFactory,
            IParser<TIdType> queryParser)
        {
            _connectionFactory = connectionFactory;
            _queryParser = queryParser;
        }

        public virtual TEntity GetById(TIdType id)
        {
            if (id != null)
            {
                ParsedQuery query = _queryParser.Select<TEntity>();
                query = _queryParser.Where<TEntity>(query, e => e.Id.Equals(id));

                using (IDbConnection sqlConnection = _connectionFactory.GetConnection())
                {
                    return sqlConnection
                        .Query<TEntity>(query.Query.ToString(), query.Parameters)
                        .FirstOrDefault();
                }
            }

            return default(TEntity);
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return GetAll(ColumnTypes.All);
        }

        public virtual IEnumerable<TEntity> GetAll(ColumnTypes columns)
        {
            ParsedQuery query = _queryParser.Select<TEntity>(columns);

            using (IDbConnection sqlConnection = _connectionFactory.GetConnection())
            {
                return sqlConnection
                    .Query<TEntity>(query.Query.ToString(), query.Parameters)
                    .ToList();
            }
        }

        public virtual bool Exists(TIdType id)
        {
            TEntity result = GetById(id);
            return result == null;
        }
    }
}

