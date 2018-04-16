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
    {
        private const string SelectAllColumns = "*";
        private const string WhereFilterKeyword = "WHERE";
        private const string AndFilterKeyword = "AND";
        private const string OrFilterKeyword = "OR";

        protected override string EscapeFormat => "[{0}]";

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
                    selectColumns = string.Join(", ", GetEntityColumnNames<TEntity>(false));
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
            string tableName = GetEntityTableName<TEntity>();
            ParsedQuery query = new ParsedQuery();
            StringBuilder queryBuilder = new StringBuilder();

            queryBuilder.AppendLine($"INSERT INTO {tableName}");
            IEnumerable<string> columnNames = GetEntityColumnNames<TEntity>(includePrimaryKey);
            string columnsToInsert = string.Format("({0})", string.Join(", ", columnNames));
            queryBuilder.AppendLine(columnsToInsert);

            // TODO: Finish logic for inserting fields - get dynamic parameters for insertions

            query.Query = queryBuilder;
            return query;
        }

        public override ParsedQuery Update<TEntity>()
        {
            return null;
        }

        public override ParsedQuery Delete<TEntity>()
        {
            return null;
        }

        public override ParsedQuery Where<TEntity>(ParsedQuery query, string paramaterNameToFilter, object value)
        {
            return ProcessFilter<TEntity>(WhereFilterKeyword, query, paramaterNameToFilter, value);
            
        }

        public override ParsedQuery And<TEntity>(ParsedQuery query, string paramaterNameToFilter, object value)
        {
            return ProcessFilter<TEntity>(AndFilterKeyword, query, paramaterNameToFilter, value);
        }

        public override ParsedQuery Or<TEntity>(ParsedQuery query, string paramaterNameToFilter, object value)
        {
            return ProcessFilter<TEntity>(OrFilterKeyword, query, paramaterNameToFilter, value);
        }

        protected virtual ParsedQuery ProcessFilter<TEntity>(string filterKeyword, ParsedQuery query, 
            string paramaterNameToFilter, object value)
            where TEntity : IEntity<TIdType>
        {
            if (query == null)
            {
                query = new ParsedQuery();
            }

            string columnName = GetColumnNameFromPropertyName<TEntity>(paramaterNameToFilter);

            query.Query.AppendLine($"{filterKeyword} {columnName} = @{columnName}");
            query.Parameters.Add($@"{columnName}", value);

            return query;
        }
    }
}
