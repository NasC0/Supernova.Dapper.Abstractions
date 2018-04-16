using Supernova.Dapper.Core.Entities;
using Supernova.Dapper.Parser.Core.Enums;
using Supernova.Dapper.Parser.Core.Models;

namespace Supernova.Dapper.Parser.Core
{
    public interface IParser<TIdType>
    {
        ParsedQuery Select<TEntity>() where TEntity : IEntity<TIdType>;

        ParsedQuery Select<TEntity>(ColumnTypes columns) where TEntity : IEntity<TIdType>;

        ParsedQuery Insert<TEntity>(TEntity entity, bool includePrimaryKey) where TEntity : IEntity<TIdType>;

        ParsedQuery Update<TEntity>(TEntity entity) where TEntity : IEntity<TIdType>;

        ParsedQuery Delete<TEntity>(TIdType id) where TEntity : IEntity<TIdType>;

        ParsedQuery Where<TEntity>(ParsedQuery query, string paramaterNameToFilter, object value) where TEntity : IEntity<TIdType>;

        ParsedQuery And<TEntity>(ParsedQuery query, string paramaterNameToFilter, object value) where TEntity : IEntity<TIdType>;

        ParsedQuery Or<TEntity>(ParsedQuery query, string paramaterNameToFilter, object value) where TEntity : IEntity<TIdType>;

        string GetEntityTableName<TEntity>() where TEntity : IEntity<TIdType>;

        string GetColumnNameFromPropertyName<TEntity>(string propertyName)
            where TEntity : IEntity<TIdType>;
    }
}