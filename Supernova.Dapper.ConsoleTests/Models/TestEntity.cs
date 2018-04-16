using System;
using Supernova.Dapper.Core.Attributes;
using Supernova.Dapper.Core.Entities;

namespace Supernova.Dapper.ConsoleTests.Models
{
    [TableName("FirstTable")]
    public class TestEntity : IEntity<int>
    {
        [PrimaryKey("TableId")]
        public int Id { get; set; }

        public string TextField { get; set; }

        [Column("DateTime")]
        public DateTime SomeDateTimeField { get; set; }

        [Column("MuchoGuid")]
        public Guid SomeGuidField { get; set; }
    }
}