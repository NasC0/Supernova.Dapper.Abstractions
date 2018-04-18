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
using Supernova.Dapper.Parser.Core.Models;
using Supernova.Dapper.Parser.SqlServer;

namespace Supernova.Dapper.ConsoleTests
{
    public class Program
    {
        private static string GetSomeValue()
        {
            return "teehee";
        }

        public static void Main()
        {
            DapperStartupMapping.RegisterCustomMaps("Supernova.Dapper.ConsoleTests.Models");
            IConnectionFactory connectionFactory = new ConnectionFactory("DefaultConnection");
            IParser<int> parser = new SqlParser<int>();

            using (IDbConnection connection = connectionFactory.GetConnection())
            {
                ParsedQuery selectAll = parser.Select<TestEntity>();
                selectAll = parser.Where<TestEntity>(selectAll, e => e.Id == 1);
                selectAll = parser.And<TestEntity>(selectAll, e => e.SomeDateTimeField < DateTime.Now);
                List<TestEntity> result = connection.Query<TestEntity>(selectAll.Query.ToString(), selectAll.Parameters)
                    .ToList();
                Console.WriteLine(result.Count);

                //TestEntity entity = new TestEntity
                //{
                //    SomeDateTimeField = DateTime.Now,
                //    SomeGuidField = Guid.NewGuid(),
                //    TextField = "this is the end, my beautiful friend, the end"
                //};

                //var allEntities = repository
                //    .GetAll()
                //    .ToList();

                //allEntities[5].TextField = "this is";
                //allEntities[6].TextField = "impossibru!";

                //repository.BulkUpdate(allEntities);

                //repository.BulkDelete(new List<int>
                //{
                //    9,
                //    10
                //});

                //List<TestEntity> entitiesToDelete = new List<TestEntity>
                //{
                //    allEntities[12],
                //    allEntities[13]
                //};

                //repository.BulkDelete(entitiesToDelete);
            }
        }
    }
}