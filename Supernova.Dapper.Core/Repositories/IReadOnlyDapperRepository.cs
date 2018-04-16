using System.Collections.Generic;
using Supernova.Dapper.Core.Entities;

namespace Supernova.Dapper.Core.Repositories
{
    public interface IReadOnlyDapperRepository<in TIdType, out TResultEntity> 
        where TResultEntity : IEntity<TIdType>
    {
        TResultEntity GetById(TIdType id);

        IEnumerable<TResultEntity> GetAll();

        bool Exists(TIdType id);
    }
}