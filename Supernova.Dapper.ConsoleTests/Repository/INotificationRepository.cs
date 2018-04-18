using System.Collections.Generic;
using Supernova.Dapper.ConsoleTests.Models;
using Supernova.Dapper.Core.Enums;
using Supernova.Dapper.Core.Repositories;

namespace Supernova.Dapper.ConsoleTests.Repository
{
    public interface INotificationRepository
        : IReadWriteDapperRepository<int, NotificationEntity>
    {
        IEnumerable<NotificationEntity> GetNotificationsWithoutCustomerIds();
    }
}
