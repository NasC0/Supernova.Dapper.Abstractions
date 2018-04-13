using System.Data;

namespace Supernova.Dapper.Core.Factories
{
    public interface IConnectionFactory
    {
        IDbConnection GetConnection();

        IDbConnection GetConnection(string connectionString);
    }
}
