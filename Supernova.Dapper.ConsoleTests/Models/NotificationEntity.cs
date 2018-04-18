using Supernova.Dapper.Core.Attributes;
using Supernova.Dapper.Core.Entities;

namespace Supernova.Dapper.ConsoleTests.Models
{
    [TableName("Notification")]
    public class NotificationEntity : IEntity<int>
    {
        [PrimaryKey]
        public int Id { get; set; }

        public string Payload { get; set; }

        public int? CustomerDataId { get; set; }
    }
}
