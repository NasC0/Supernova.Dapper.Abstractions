using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Supernova.Dapper.ConsoleTests.Models;
using Supernova.Dapper.Core.Factories;
using Supernova.Dapper.Factories;
using Supernova.Dapper.Initialization;
using Supernova.Dapper.Parser.Core;
using Supernova.Dapper.Parser.Core.Enums;
using Supernova.Dapper.Parser.Core.Models;
using Supernova.Dapper.Parser.SqlServer;

namespace Supernova.Dapper.ConsoleTests
{
    public class Program
    {
        public static void Main()
        {
            DapperStartupMapping.RegisterCustomMaps("Supernova.Dapper.ConsoleTests.Models");
            IConnectionFactory connectionFactory = new ConnectionFactory("DefaultConnection");
            IParser<int> parser = new SqlParser<int>();
            ParsedQuery getAll = parser.Select<TestEntity>(ColumnTypes.EntityColumns);

            using (IDbConnection connection = connectionFactory.GetConnection())
            {
                TestEntity currentEntry = connection
                    .Query<TestEntity>(getAll.Query.ToString(), getAll.Parameters)
                    .FirstOrDefault();
            }
        }
    }
}