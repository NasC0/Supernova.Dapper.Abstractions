using System.Collections.Generic;
using Supernova.Dapper.Core.Entities;
using Supernova.Dapper.Core.Enums;

namespace Supernova.Dapper.Core.Repositories
{
    public interface IReadOnlyDapperRepository<in TIdType, out TResultEntity> 
        where TResultEntity : IEntity<TIdType>
    {
        TResultEntity GetById(TIdType id);

        IEnumerable<TResultEntity> GetAll();

        IEnumerable<TResultEntity> GetAll(ColumnTypes columns);

        bool Exists(TIdType id);
    }
}