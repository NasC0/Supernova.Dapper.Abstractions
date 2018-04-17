using System;
using System.Collections.Generic;
using Supernova.Dapper.ConsoleTests.Models;
using Supernova.Dapper.Parser.Core;
using Supernova.Dapper.Core.Factories;
using Supernova.Dapper.Base;

namespace Supernova.Dapper.ConsoleTests.Repository
{
    public class TestRepository : ReadWriteDapperRepository<int, TestEntity>
    {
        public TestRepository(IConnectionFactory connectionFactory, IParser<int> queryParser) 
            : base(connectionFactory, queryParser)
        {
        }
    }
}
