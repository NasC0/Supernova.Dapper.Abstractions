﻿using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
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

        public virtual void BulkInsert(IEnumerable<TEntity> entities)
        {
            StringBuilder bulkQuery = new StringBuilder();
            DynamicParameters bulkParameters = new DynamicParameters();

            int insertCount = 1;
            foreach (TEntity entity in entities)
            {
                ParsedQuery query = _queryParser.Insert(entity, false, insertCount.ToString());
                bulkQuery.Append(query.Query);
                bulkParameters.AddDynamicParams(query.Parameters);
                insertCount++;
            }

            using (IDbConnection connection = _connectionFactory.GetConnection())
            {
                connection.Query<TEntity>(bulkQuery.ToString(), bulkParameters);
            }
        }

        public virtual void Update(TEntity update)
        {
            ParsedQuery query = _queryParser.Update(update);
            using (IDbConnection sqlConnection = _connectionFactory.GetConnection())
            {
                sqlConnection.Query(query.Query.ToString(), query.Parameters);
            }
        }

        public virtual void BulkUpdate(IEnumerable<TEntity> entities)
        {
            StringBuilder bulkQuery = new StringBuilder();
            DynamicParameters bulkParameters = new DynamicParameters();

            int insertCount = 1;
            foreach (TEntity entity in entities)
            {
                ParsedQuery query = _queryParser
                    .Update(entity, insertCount.ToString());
                bulkQuery.Append(query.Query);
                bulkParameters.AddDynamicParams(query.Parameters);
                insertCount++;
            }

            using (IDbConnection connection = _connectionFactory.GetConnection())
            {
                connection.Query<TEntity>(bulkQuery.ToString(), bulkParameters);
            }
        }

        public virtual void Delete(TIdType id)
        {
            ParsedQuery query = _queryParser.Delete<TEntity>(id);
            using (IDbConnection sqlConnection = _connectionFactory.GetConnection())
            {
                sqlConnection.Query(query.Query.ToString(), query.Parameters);
            }
        }

        public virtual void BulkDelete(IEnumerable<TIdType> ids)
        {
            StringBuilder bulkQuery = new StringBuilder();
            DynamicParameters bulkParameters = new DynamicParameters();

            int insertCount = 1;
            foreach (TIdType id in ids)
            {
                ParsedQuery query = _queryParser.Delete<TEntity>(id, insertCount.ToString());
                bulkQuery.Append(query.Query);
                bulkParameters.AddDynamicParams(query.Parameters);
                insertCount++;
            }

            using (IDbConnection sqlConnection = _connectionFactory.GetConnection())
            {
                sqlConnection.Query<TEntity>(bulkQuery.ToString(), bulkParameters);
            }
        }

        public virtual void BulkDelete(IEnumerable<TEntity> entities)
        {
            List<TIdType> entityIds = entities
                .Select(e => e.Id)
                .ToList();

            BulkDelete(entityIds);
        }
    }
}
