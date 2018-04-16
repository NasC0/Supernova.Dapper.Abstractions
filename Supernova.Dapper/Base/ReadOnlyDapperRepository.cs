using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Supernova.Dapper.Core.Factories;
using Supernova.Dapper.Core.Repositories;

namespace Supernova.Dapper.Base
{
    public abstract class ReadOnlyDapperRepository<TIdType, TResultEntity> : 
        IReadOnlyDapperRepository<TIdType, TResultEntity>
    {
        protected readonly IConnectionFactory _connectionFactory;

        protected ReadOnlyDapperRepository(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public virtual string TableName { get; set; }

        public virtual TResultEntity GetById(TIdType id)
        {
            string selectionSql = $"SELECT * FROM " +
                                  $"{TableName} " +
                                  $"WHERE Id = @Id";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);

            using (IDbConnection sqlConnection = _connectionFactory.GetConnection())
            {
                return sqlConnection
                    .Query<TResultEntity>(selectionSql, parameters)
                    .FirstOrDefault();
            }
        }

        public virtual IEnumerable<TResultEntity> GetAll()
        {
            string selectionSql = "SELECT * " +
                                  $"FROM {TableName}";

            using (IDbConnection sqlConnection = _connectionFactory.GetConnection())
            {
                return sqlConnection
                    .Query<TResultEntity>(selectionSql)
                    .ToList();
            }
        }

        public virtual bool Exists(TIdType id)
        {
            string selectionSql = $"SELECT Count(Id) FROM " +
                                  $"{TableName} " +
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

