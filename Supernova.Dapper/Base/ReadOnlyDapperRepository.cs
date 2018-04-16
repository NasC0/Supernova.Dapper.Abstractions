using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Supernova.Dapper.Core.Entities;
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
            ParsedQuery query = _queryParser.Select<TEntity>();
            query = _queryParser.Where<TEntity>(query, nameof(IEntity<TIdType>.Id), id);

            using (IDbConnection sqlConnection = _connectionFactory.GetConnection())
            {
                return sqlConnection
                    .Query<TEntity>(query.ToString(), query.Parameters)
                    .FirstOrDefault();
            }
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            ParsedQuery query = _queryParser.Select<TEntity>();

            using (IDbConnection sqlConnection = _connectionFactory.GetConnection())
            {
                return sqlConnection
                    .Query<TEntity>(query.Query.ToString(), query.Parameters)
                    .ToList();
            }
        }

        public virtual bool Exists(TIdType id)
        {
            string tableName = _queryParser.GetEntityTableName<TEntity>();
            string idParameterName = 
                _queryParser.GetColumnNameFromPropertyName<TEntity>(nameof(IEntity<TIdType>.Id));

            string selectionSql = $"SELECT Count({idParameterName}) FROM " +
                                  tableName +
                                  $"WHERE Id = @Id";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);

            using (IDbConnection sqlConnection = _connectionFactory.GetConnection())
            {
                int count = sqlConnection.ExecuteScalar<int>(selectionSql, parameters);
                bool recordExists = count > 0;
                return recordExists;
            }
        }
    }
}

