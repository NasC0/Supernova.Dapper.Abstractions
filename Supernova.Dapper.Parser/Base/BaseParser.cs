using System;
using System.Collections.Generic;
using System.Reflection;
using Supernova.Dapper.Core.Attributes;
using Supernova.Dapper.Core.Entities;
using Supernova.Dapper.Parser.Core;
using Supernova.Dapper.Parser.Core.Enums;
using Supernova.Dapper.Parser.Core.Models;

namespace Supernova.Dapper.Parser.Base
{
    public abstract class BaseParser<TIdType> : IParser<TIdType>
    {
        protected virtual string EscapeFormat => string.Empty;

        public abstract ParsedQuery Select<TEntity>() where TEntity : IEntity<TIdType>;

        public abstract ParsedQuery Select<TEntity>(ColumnTypes columns) where TEntity : IEntity<TIdType>;
        
        public abstract ParsedQuery Insert<TEntity>(TEntity entity, bool includePrimaryKey) 
            where TEntity : IEntity<TIdType>;

        public abstract ParsedQuery Update<TEntity>() where TEntity : IEntity<TIdType>;

        public abstract ParsedQuery Delete<TEntity>() where TEntity : IEntity<TIdType>;

        public abstract ParsedQuery Where<TEntity>(ParsedQuery query, string paramaterNameToFilter, object value) where TEntity : IEntity<TIdType>;

        public abstract ParsedQuery And<TEntity>(ParsedQuery query, string paramaterNameToFilter, object value) where TEntity : IEntity<TIdType>;

        public abstract ParsedQuery Or<TEntity>(ParsedQuery query, string paramaterNameToFilter, object value) where TEntity : IEntity<TIdType>;

        public virtual string GetEntityTableName<TEntity>() where TEntity : IEntity<TIdType>
        {
            Type entityType = typeof(TEntity);
            TableNameAttribute tableNameAttribute = 
                entityType.GetCustomAttribute<TableNameAttribute>();

            if (!string.IsNullOrWhiteSpace(tableNameAttribute?.Name))
            {
                if (!string.IsNullOrWhiteSpace(EscapeFormat))
                {
                    return string.Format(EscapeFormat, tableNameAttribute.Name);
                }
            }

            throw new InvalidOperationException($"Missing tablename attribute for entity {entityType.Name} in {entityType.Namespace} namespace");
        }

        protected virtual IEnumerable<string> GetEntityColumnNames<TEntity>(bool includePrimaryKey)
            where TEntity : IEntity<TIdType>
        {
            Type entityType = typeof(TEntity);
            IEnumerable<PropertyInfo> entityProperties = entityType.GetRuntimeProperties();
            List<string> columnNames = new List<string>();
            foreach (PropertyInfo property in entityProperties)
            {
                columnNames.Add(GetColumnNameFromProperty(property, includePrimaryKey));
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

            if ((attribute is PrimaryKeyAttribute && includePrimaryKey)
                || attribute is ColumnAttribute)
            {
                if (!string.IsNullOrWhiteSpace(attribute.Name))
                {
                    columnName = attribute.Name;
                }
            }

            if (!string.IsNullOrWhiteSpace(EscapeFormat))
            {
                columnName = string.Format(EscapeFormat, columnName);
            }

            return columnName;
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
    }
}