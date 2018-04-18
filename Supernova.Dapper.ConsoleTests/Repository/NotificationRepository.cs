using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Supernova.Dapper.Base;
using Supernova.Dapper.ConsoleTests.Models;
using Supernova.Dapper.Core.Enums;
using Supernova.Dapper.Core.Factories;
using Supernova.Dapper.Parser.Core;
using Supernova.Dapper.Parser.Core.Models;

namespace Supernova.Dapper.ConsoleTests.Repository
{
    public class NotificationRepository : ReadWriteDapperRepository<int, NotificationEntity>,
        INotificationRepository
    {
        public NotificationRepository(IConnectionFactory connectionFactory, IParser<int> queryParser)
            : base(connectionFactory, queryParser)
        {

        }

        public IEnumerable<NotificationEntity> GetNotificationsWithoutCustomerIds()
        {
            ParsedQuery query = _queryParser.Select<NotificationEntity>(ColumnTypes.EntityColumns);
            query = _queryParser.Where<NotificationEntity>(query, e => e.CustomerDataId == null);

            using (IDbConnection sqlConnection = _connectionFactory.GetConnection())
            {
                return sqlConnection
                    .Query<NotificationEntity>(query.Query.ToString(), query.Parameters)
                    .ToList();
            }
        }
    }
}
