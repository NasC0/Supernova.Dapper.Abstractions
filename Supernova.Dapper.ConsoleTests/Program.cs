using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
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
            TestEntity maznotoEntity = new TestEntity
            {
                Id = 3
            };

            var parsedQuery = parser.Where<TestEntity>(new ParsedQuery(), e => e.TextField != "teehee");

            using (IDbConnection connection = connectionFactory.GetConnection())
            {
                ParsedQuery selectAll = parser.Select<TestEntity>();
                selectAll = parser.Where<TestEntity>(selectAll, e => e.TextField != "teehee");
                var result = connection.Query<TestEntity>(selectAll.Query.ToString(), selectAll.Parameters)
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