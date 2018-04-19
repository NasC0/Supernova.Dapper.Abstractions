using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Newtonsoft.Json.Linq;
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
        public const int ReadBatchSize = 1000;
        public const int WriteBatchSize = 100;

        private static readonly INotificationRepository _notificationRepository = InitializeRepository();

        private static INotificationRepository InitializeRepository()
        {
            DapperStartupMapping.RegisterCustomMaps("CrmBackend.UtilityTools.MigrateNotifications.Models");
            IConnectionFactory factory = new ConnectionFactory("CrmConnection");
            IParser<int> parser = new SqlParser<int>();
            NotificationRepository repository = new NotificationRepository(factory, parser);
            return repository;
        }

        private static void UpdateNotifications(IEnumerable<NotificationEntity> notifications)
        {
            List<NotificationEntity> materializedNotifications = notifications.ToList();
            int currentSkip = 0;

            while (true)
            {
                Console.WriteLine($"Updating batch {currentSkip} to {currentSkip + WriteBatchSize}");
                List<NotificationEntity> currentNotificationBatch = materializedNotifications
                    .Skip(currentSkip)
                    .Take(WriteBatchSize)
                    .ToList();

                _notificationRepository.BulkUpdate(currentNotificationBatch,
                    nameof(NotificationEntity.Payload));

                if (currentNotificationBatch.Count < WriteBatchSize)
                {
                    break;
                }

                currentSkip += WriteBatchSize;
            }
        }

        public static void Main()
        {
            //Console.WriteLine("Fetching notifications from database.");
            //List<NotificationEntity> result = _notificationRepository
            //    .GetNotificationsWithoutCustomerIds()
            //    .ToList();

            //Console.WriteLine($"{result.Count} notifications fetched.");

            //Console.WriteLine("Parsing JSON payload.");
            //foreach (NotificationEntity notificationEntity in result)
            //{
            //    JObject payloadObject = JObject.Parse(notificationEntity.Payload);
            //    int customerDataId = int.Parse(payloadObject["customerDataId"].ToString());
            //    notificationEntity.CustomerDataId = customerDataId;
            //}

            //Console.WriteLine("Parsed JSON payload");

            //UpdateNotifications(result);
            DapperStartupMapping.RegisterCustomMaps("Supernova.Dapper.ConsoleTests.Models");
            IConnectionFactory connectionFactory = new ConnectionFactory("DefaultConnection");
            IParser<int> parser = new SqlParser<int>();
            TestRepository repository = new TestRepository(connectionFactory, parser);

            using (IDbConnection connection = connectionFactory.GetConnection())
            {
                ParsedQuery selectAll = parser.Select<TestEntity>();
                selectAll = parser.Where<TestEntity>(selectAll, e => e.Id == 1);
                selectAll = parser.And<TestEntity>(selectAll, e => e.SomeDateTimeField < DateTime.Now);
                List<TestEntity> result = connection.Query<TestEntity>(selectAll.Query.ToString(), selectAll.Parameters)
                    .ToList();
                Console.WriteLine(result.Count);

                TestEntity entity = new TestEntity
                {
                    SomeDateTimeField = DateTime.Now,
                    SomeGuidField = Guid.NewGuid(),
                    TextField = "this is the end, my beautiful friend, the end"
                };

                repository.Insert(null);

                var allEntities = repository
                    .GetAll()
                    .ToList();

                foreach (TestEntity testEntity in allEntities)
                {
                    testEntity.TextField = "teehee";
                    testEntity.SomeDateTimeField = new DateTime(2010, 2, 2);
                }

                repository.BulkUpdate(allEntities, nameof(TestEntity.SomeDateTimeField));
            }
        }
    }
}