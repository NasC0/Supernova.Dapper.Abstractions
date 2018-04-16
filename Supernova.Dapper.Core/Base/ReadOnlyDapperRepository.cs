using System;
using System.Collections.Generic;
using System.Data;
using Supernova.Dapper.Core.Entities;
using Supernova.Dapper.Core.Factories;
using Supernova.Dapper.Core.Repositories;

namespace Supernova.Dapper.Core.Base
{
    public abstract class ReadOnlyDapperRepository<TIdType, TResultEntity> : 
        IReadOnlyDapperRepository<TIdType, TResultEntity>
        where TIdType : new()
        where TResultEntity : IEntity<TIdType>
    {
        private readonly IConnectionFactory _connectionFactory;

        protected ReadOnlyDapperRepository(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public virtual TResultEntity GetById(TIdType id)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<TResultEntity> GetAll()
        {
            throw new NotImplementedException();
        }

        public virtual bool Exists(TIdType id)
        {
            throw new NotImplementedException();
        }
    }
}
