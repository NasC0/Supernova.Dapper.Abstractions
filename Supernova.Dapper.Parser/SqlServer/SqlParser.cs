using System.Collections.Generic;
using System.Text;
using Dapper;
using Supernova.Dapper.Core.Entities;
using Supernova.Dapper.Parser.Base;
using Supernova.Dapper.Parser.Core.Enums;
using Supernova.Dapper.Parser.Core.Models;

namespace Supernova.Dapper.Parser.SqlServer
{
    public class SqlParser<TIdType> : BaseParser<TIdType>
        where TIdType : new()
    {
        private const string SelectAllColumns = "*";
        private const string WhereFilterKeyword = "WHERE";
        private const string AndFilterKeyword = "AND";
        private const string OrFilterKeyword = "OR";

        public override ParsedQuery Select<TEntity>()
        {
            return Select<TEntity>(ColumnTypes.All);
        }

        public override ParsedQuery Select<TEntity>(ColumnTypes columns)
        {
            string selectColumns;
            switch (columns)
            {
                case ColumnTypes.EntityColumns:
                    selectColumns = string.Join(", ", GetEntityColumnNames<TEntity>(true));
                    break;
                default:
                    selectColumns = SelectAllColumns;
                    break;
            }

            string tableName = GetEntityTableName<TEntity>();
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.AppendLine($"SELECT {selectColumns} FROM {tableName}");

            return new ParsedQuery
            {
                Query = queryBuilder
            };
        }

        public override ParsedQuery Insert<TEntity>(TEntity entity, bool includePrimaryKey)
        {
            return Insert(entity, includePrimaryKey, string.Empty);
        }

        public override ParsedQuery Insert<TEntity>(TEntity entity, bool includePrimaryKey, string seedParameter)
        {
            string tableName = GetEntityTableName<TEntity>();
            ParsedQuery query = new ParsedQuery();
            StringBuilder queryBuilder = new StringBuilder();

            queryBuilder.AppendLine($"INSERT INTO {tableName}");

            IEnumerable<string> columnNames = GetEntityColumnNames<TEntity>(includePrimaryKey);
            string columnsToInsert = string.Format("({0})", string.Join(", ", columnNames));
            queryBuilder.AppendLine(columnsToInsert);

            DynamicParameters parameters = GetEntityParameters(entity, includePrimaryKey, seedParameter);
            string joinedParameters = "@" + string.Join(", @", parameters.ParameterNames);
            queryBuilder.AppendLine(string.Format("VALUES ({0})", joinedParameters));

            query.Query = queryBuilder;
            query.Parameters = parameters;
            return query;
        }

        public override ParsedQuery Update<TEntity>(TEntity entity)
        {
            return Update(entity, string.Empty);
        }

        public override ParsedQuery Update<TEntity>(TEntity entity, string seedParameter)
        {
            string tableName = GetEntityTableName<TEntity>();
            ParsedQuery query = new ParsedQuery();
            StringBuilder queryBuilder = new StringBuilder();

            queryBuilder.AppendLine($"UPDATE {tableName}");
            IEnumerable<string> columnNames = GetEntityColumnNames<TEntity>(false);
            List<string> parameterPairs = new List<string>();

            foreach (string columnName in columnNames)
            {
                parameterPairs.Add(string.Format("{0} = @{0}{1}", columnName, seedParameter));
            }

            queryBuilder.AppendLine(string.Format("SET {0}", string.Join(", ", parameterPairs)));
            DynamicParameters parameters = GetEntityParameters(entity, false, seedParameter);

            query.Query = queryBuilder;
            query.Parameters = parameters;

            query = Where<TEntity>(query, nameof(IEntity<TIdType>.Id), entity.Id, seedParameter);
            return query;
        }

        public override ParsedQuery Delete<TEntity>(TIdType id)
        {
            return Delete<TEntity>(id, string.Empty);
        }

        public override ParsedQuery Delete<TEntity>(TIdType id, string seedParameter)
        {
            ParsedQuery query = Where<TEntity>(null, nameof(IEntity<TIdType>.Id), id, seedParameter);
            string tableName = GetEntityTableName<TEntity>();
            query.Query.Insert(0, $"DELETE FROM {tableName} ");

            return query;
        }

        public override ParsedQuery Where<TEntity>(ParsedQuery query, string paramaterNameToFilter, object value)
        {
            return Where<TEntity>(query, paramaterNameToFilter, value, string.Empty);
        }

        public override ParsedQuery Where<TEntity>(ParsedQuery query, string paramaterNameToFilter, object value, string parameterSeed)
        {
            return ProcessFilter<TEntity>(WhereFilterKeyword, query, paramaterNameToFilter, value, parameterSeed);
        }

        public override ParsedQuery And<TEntity>(ParsedQuery query, string paramaterNameToFilter, object value)
        {
            return And<TEntity>(query, paramaterNameToFilter, value, string.Empty);
        }

        public override ParsedQuery And<TEntity>(ParsedQuery query, string paramaterNameToFilter, object value, string parameterSeed)
        {
            return ProcessFilter<TEntity>(AndFilterKeyword, query, paramaterNameToFilter, value, parameterSeed);
        }

        public override ParsedQuery Or<TEntity>(ParsedQuery query, string paramaterNameToFilter, object value)
        {
            return Or<TEntity>(query, paramaterNameToFilter, value, string.Empty);
        }

        public override ParsedQuery Or<TEntity>(ParsedQuery query, string paramaterNameToFilter, object value, string parameterSeed)
        {
            return ProcessFilter<TEntity>(OrFilterKeyword, query, paramaterNameToFilter, value, parameterSeed);
        }

        protected virtual ParsedQuery ProcessFilter<TEntity>(string filterKeyword, ParsedQuery query, 
            string paramaterNameToFilter, object value, string parameterSeed)
            where TEntity : IEntity<TIdType>
        {
            if (query == null)
            {
                query = new ParsedQuery();
            }

            string columnName = GetColumnNameFromPropertyName<TEntity>(paramaterNameToFilter);

            query.Query.AppendLine($"{filterKeyword} {columnName} = @{columnName}{parameterSeed}");
            query.Parameters.Add($"@{columnName}{parameterSeed}", value);

            return query;
        }
    }
}
