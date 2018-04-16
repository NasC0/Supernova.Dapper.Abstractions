using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Supernova.Dapper.ConsoleTests.Models;
using Supernova.Dapper.ConsoleTests.Repository;
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
            TestRepository repository = new TestRepository(connectionFactory, parser);


            using (IDbConnection connection = connectionFactory.GetConnection())
            {
                TestEntity entity = new TestEntity
                {
                    SomeDateTimeField = DateTime.Now,
                    SomeGuidField = Guid.NewGuid(),
                    TextField = "this is the end, my beautiful friend, the end"
                };

                var allEntities = repository.GetAll();
                repository.Insert(entity);

                var currentEntity = repository.GetById(3);
                currentEntity.TextField = "teehee";

                repository.Update(currentEntity);

                repository.Delete(3);
            }
        }
    }
}