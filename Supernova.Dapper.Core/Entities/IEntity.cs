using System;

namespace Supernova.Dapper.Core.Entities
{
    public interface IEntity<TIdType>
    {
        TIdType Id { get; set; }
    }
}