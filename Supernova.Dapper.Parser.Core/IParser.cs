using System;
using System.Linq.Expressions;
using Supernova.Dapper.Core.Entities;
using Supernova.Dapper.Core.Enums;
using Supernova.Dapper.Parser.Core.Models;

namespace Supernova.Dapper.Parser.Core
{
    public interface IParser<TIdType>
    {
        ParsedQuery Select<TEntity>() where TEntity : IEntity<TIdType>;

        ParsedQuery Select<TEntity>(ColumnTypes columns) where TEntity : IEntity<TIdType>;

        ParsedQuery Insert<TEntity>(TEntity entity)
            where TEntity : IEntity<TIdType>;

        ParsedQuery Insert<TEntity>(TEntity entity, bool includePrimaryKey) where TEntity : IEntity<TIdType>;

        ParsedQuery Insert<TEntity>(TEntity entity, bool includePrimaryKey, params string[] ignoreColumns) 
            where TEntity : IEntity<TIdType>;

        ParsedQuery Insert<TEntity>(TEntity entity, bool includePrimaryKey, string seedParameter) 
            where TEntity : IEntity<TIdType>;

        ParsedQuery Insert<TEntity>(TEntity entity, bool includePrimaryKey, string seedParameter, params string[] ignoreColumns)
            where TEntity : IEntity<TIdType>;

        ParsedQuery Update<TEntity>(TEntity entity) where TEntity : IEntity<TIdType>;

        ParsedQuery Update<TEntity>(TEntity entity, params string[] ignoreColumns) where TEntity : IEntity<TIdType>;

        ParsedQuery Update<TEntity>(TEntity entity, string seedParameter) where TEntity : IEntity<TIdType>;

        ParsedQuery Update<TEntity>(TEntity entity, string seedParameter, params string[] ignoreColumns) 
            where TEntity : IEntity<TIdType>;

        ParsedQuery Delete<TEntity>(TIdType id) where TEntity : IEntity<TIdType>;

        ParsedQuery Delete<TEntity>(TIdType id, string seedParameter) where TEntity : IEntity<TIdType>;

        ParsedQuery Where<TEntity>(ParsedQuery query, Expression<Func<TEntity, bool>> filter) where TEntity : IEntity<TIdType>;

        ParsedQuery Where<TEntity>(ParsedQuery query, Expression<Func<TEntity, bool>> filter, string seedParameter) where TEntity : IEntity<TIdType>;

        ParsedQuery And<TEntity>(ParsedQuery query, Expression<Func<TEntity, bool>> filter) where TEntity : IEntity<TIdType>;

        ParsedQuery And<TEntity>(ParsedQuery query, Expression<Func<TEntity, bool>> filter, string seedParameter) where TEntity : IEntity<TIdType>;

        ParsedQuery Or<TEntity>(ParsedQuery query, Expression<Func<TEntity, bool>> filter) where TEntity : IEntity<TIdType>;

        ParsedQuery Or<TEntity>(ParsedQuery query, Expression<Func<TEntity, bool>> filter, string seedParameter) where TEntity : IEntity<TIdType>;

        string GetEntityTableName<TEntity>() where TEntity : IEntity<TIdType>;

        string GetColumnNameFromPropertyName<TEntity>(string propertyName)
            where TEntity : IEntity<TIdType>;
    }
}