using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dapper;
using Supernova.Dapper.Core.Attributes;
using Supernova.Dapper.Core.Entities;
using Supernova.Dapper.Core.Enums;
using Supernova.Dapper.Core.Extensions;
using Supernova.Dapper.Parser.Core;
using Supernova.Dapper.Parser.Core.Models;

namespace Supernova.Dapper.Parser.Base
{
    public abstract class BaseParser<TIdType> : IParser<TIdType>
    {
        protected string _tableName;

        public abstract ParsedQuery Select<TEntity>() where TEntity : IEntity<TIdType>;

        public abstract ParsedQuery Select<TEntity>(ColumnTypes columns) where TEntity : IEntity<TIdType>;

        public abstract ParsedQuery Insert<TEntity>(TEntity entity)
            where TEntity : IEntity<TIdType>;
        
        public abstract ParsedQuery Insert<TEntity>(TEntity entity, bool includePrimaryKey) 
            where TEntity : IEntity<TIdType>;
        
        public abstract ParsedQuery Insert<TEntity>(TEntity entity, bool includePrimaryKey, params string[] ignoreColumns)
            where TEntity : IEntity<TIdType>;

        public abstract ParsedQuery Insert<TEntity>(TEntity entity, bool includePrimaryKey, string seedParameter) 
            where TEntity : IEntity<TIdType>;

        public abstract ParsedQuery Insert<TEntity>(TEntity entity, bool includePrimaryKey, string seedParameter, params string[] ignoreColumns)
            where TEntity : IEntity<TIdType>;

        public abstract ParsedQuery Update<TEntity>(TEntity entity) where TEntity : IEntity<TIdType>;

        public abstract ParsedQuery Update<TEntity>(TEntity entity, params string[] ignoreColumns) where TEntity : IEntity<TIdType>;

        public abstract ParsedQuery Update<TEntity>(TEntity entity, string seedParameter) where TEntity : IEntity<TIdType>;

        public abstract ParsedQuery Update<TEntity>(TEntity entity, string seedParameter, params string[] ignoreColumns) 
            where TEntity : IEntity<TIdType>;

        public abstract ParsedQuery Delete<TEntity>(TIdType id) where TEntity : IEntity<TIdType>;

        public abstract ParsedQuery Delete<TEntity>(TIdType id, string seedParameter) where TEntity : IEntity<TIdType>;

        public abstract ParsedQuery Where<TEntity>(ParsedQuery query, Expression<Func<TEntity, bool>> filter) where TEntity : IEntity<TIdType>;

        public abstract ParsedQuery Where<TEntity>(ParsedQuery query, Expression<Func<TEntity, bool>> filter, string parameterSeed) 
            where TEntity : IEntity<TIdType>;

        public abstract ParsedQuery And<TEntity>(ParsedQuery query, Expression<Func<TEntity, bool>> filter) where TEntity : IEntity<TIdType>;

        public abstract ParsedQuery And<TEntity>(ParsedQuery query, Expression<Func<TEntity, bool>> filter, string parameterSeed) 
            where TEntity : IEntity<TIdType>;

        public abstract ParsedQuery Or<TEntity>(ParsedQuery query, Expression<Func<TEntity, bool>> filter) where TEntity : IEntity<TIdType>;

        public abstract ParsedQuery Or<TEntity>(ParsedQuery query, Expression<Func<TEntity, bool>> filter, string parameterSeed) 
            where TEntity : IEntity<TIdType>;

        public virtual string GetEntityTableName<TEntity>() where TEntity : IEntity<TIdType>
        {
            if (string.IsNullOrWhiteSpace(_tableName))
            {
                Type entityType = typeof(TEntity);
                TableNameAttribute tableNameAttribute =
                    entityType.GetCustomAttribute<TableNameAttribute>();

                if (!string.IsNullOrWhiteSpace(tableNameAttribute?.Name))
                {
                    _tableName = tableNameAttribute.Name;
                    return _tableName;
                }

                throw new InvalidOperationException($"Missing tablename attribute for entity {entityType.Name} in {entityType.Namespace} namespace");
            }

            return _tableName;
        }

        public string GetColumnNameFromPropertyName<TEntity>(string propertyName)
            where TEntity : IEntity<TIdType>
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return null;
            }

            Type type = typeof(TEntity);
            PropertyInfo property = type.GetProperty(propertyName);
            return GetColumnNameFromProperty(property, true);
        }

        protected virtual IEnumerable<string> GetEntityColumnNames<TEntity>(bool includePrimaryKey)
            where TEntity : IEntity<TIdType>
        {
            return GetEntityColumnNames<TEntity>(includePrimaryKey, Enumerable.Empty<string>());
        }

        protected virtual IEnumerable<string> GetEntityColumnNames<TEntity>(bool includePrimaryKey,
            IEnumerable<string> ignoreColumns)
            where TEntity : IEntity<TIdType>
        {
            HashSet<string> columnSet = ignoreColumns.ToHashSet();
            Type entityType = typeof(TEntity);
            IEnumerable<PropertyInfo> entityProperties = entityType.GetRuntimeProperties();
            List<string> columnNames = new List<string>();
            foreach (PropertyInfo property in entityProperties)
            {
                bool ignoreColumn = columnSet.Contains(property.Name);
                string columnName = GetColumnNameFromProperty(property, includePrimaryKey);
                if (!string.IsNullOrWhiteSpace(columnName) && !ignoreColumn)
                {
                    columnNames.Add(columnName);
                }
            }

            return columnNames;
        }

        protected virtual string GetColumnNameFromProperty(PropertyInfo property, bool includePrimaryKey)
        {
            string columnName = property.Name;

            BaseAttribute attribute = property.GetCustomAttribute<BaseAttribute>();
            if (attribute == null)
            {
                columnName = property.Name;
            }

            if (attribute is PrimaryKeyAttribute && !includePrimaryKey)
            {
                return null;
            }

            if (attribute != null)
            {
                if (!string.IsNullOrWhiteSpace(attribute.Name))
                {
                    columnName = attribute.Name;
                }
            }

            return columnName;
        }

        protected virtual DynamicParameters GetEntityParameters<TEntity>(TEntity entity, bool includePrimaryKey, string parameterKeySeed)
            where TEntity : IEntity<TIdType>
        {
            return GetEntityParameters(entity, includePrimaryKey, 
                parameterKeySeed, Enumerable.Empty<string>());
        }

        protected virtual DynamicParameters GetEntityParameters<TEntity>(TEntity entity, bool includePrimaryKey, string parameterKeySeed, IEnumerable<string> ignoreColumns)
            where TEntity : IEntity<TIdType>
        {
            HashSet<string> columnSet = ignoreColumns.ToHashSet();
            Type entityType = entity.GetType();
            DynamicParameters parameters = new DynamicParameters();
            PropertyInfo[] properties = entityType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                bool ignoreColumn = columnSet.Contains(property.Name);
                string columnName = GetColumnNameFromProperty(property, includePrimaryKey);
                if (!string.IsNullOrWhiteSpace(columnName) && !ignoreColumn)
                {
                    string parameter = string.Format("@{0}{1}", columnName, parameterKeySeed);
                    parameters.Add(parameter, property.GetValue(entity));
                }
            }

            return parameters;
        }
    }
}