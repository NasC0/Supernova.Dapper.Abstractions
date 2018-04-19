using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Dapper;
using Supernova.Dapper.Core.Entities;
using Supernova.Dapper.Core.Enums;
using Supernova.Dapper.Parser.Base;
using Supernova.Dapper.Parser.Core;
using Supernova.Dapper.Parser.Core.Models;

namespace Supernova.Dapper.Parser.SqlServer
{
    public class SqlParser<TIdType> : BaseParser<TIdType>
    {
        private const string SelectAllColumns = "*";
        private const string WhereFilterKeyword = "WHERE";
        private const string AndFilterKeyword = "AND";
        private const string OrFilterKeyword = "OR";

        protected readonly IQueryTranslator _translator;

        public SqlParser()
            : this(new SqlServerTranslator())
        {
        }

        public SqlParser(IQueryTranslator translator)
        {
            _translator = translator;
        }

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

        public override ParsedQuery Insert<TEntity>(TEntity entity)
        {
            return Insert(entity, false);
        }

        public override ParsedQuery Insert<TEntity>(TEntity entity, bool includePrimaryKey)
        {
            return Insert(entity, includePrimaryKey, string.Empty);
        }

        public override ParsedQuery Insert<TEntity>(TEntity entity, bool includePrimarykey, params string[] ignoreColumns)
        {
            return Insert(entity, includePrimarykey, string.Empty, ignoreColumns);
        }

        public override ParsedQuery Insert<TEntity>(TEntity entity, bool includePrimaryKey, string seedParameter)
        {
            return Insert(entity, includePrimaryKey, seedParameter, new string[0]);
        }

        public override ParsedQuery Insert<TEntity>(TEntity entity, bool includePrimaryKey, string seedParameter, params string[] ignoreColumns)
        {
            if (entity != null)
            {
                string tableName = GetEntityTableName<TEntity>();
                ParsedQuery query = new ParsedQuery();
                StringBuilder queryBuilder = new StringBuilder();

                queryBuilder.AppendLine($"INSERT INTO {tableName}");

                IEnumerable<string> columnNames = GetEntityColumnNames<TEntity>(includePrimaryKey, ignoreColumns);
                string columnsToInsert = string.Format("({0})", string.Join(", ", columnNames));
                queryBuilder.AppendLine(columnsToInsert);

                DynamicParameters parameters = GetEntityParameters(entity, includePrimaryKey, seedParameter);
                string joinedParameters = "@" + string.Join(", @", parameters.ParameterNames);
                queryBuilder.AppendLine(string.Format("VALUES ({0})", joinedParameters));

                query.Query = queryBuilder;
                query.Parameters = parameters;
                return query;
            }

            return new ParsedQuery();
        }

        public override ParsedQuery Update<TEntity>(TEntity entity)
        {
            return Update(entity, string.Empty);
        }

        public override ParsedQuery Update<TEntity>(TEntity entity, params string[] ignoreColumns)
        {
            return Update(entity, string.Empty, ignoreColumns);
        }

        public override ParsedQuery Update<TEntity>(TEntity entity, string seedParameter)
        {
            return Update(entity, seedParameter, new string[0]);
        }

        public override ParsedQuery Update<TEntity>(TEntity entity, string seedParameter, params string[] ignoreColumns)
        {
            if (entity != null)
            {
                string tableName = GetEntityTableName<TEntity>();
                ParsedQuery query = new ParsedQuery();
                StringBuilder queryBuilder = new StringBuilder();

                queryBuilder.AppendLine($"UPDATE {tableName}");
                IEnumerable<string> columnNames = GetEntityColumnNames<TEntity>(false, ignoreColumns);
                List<string> parameterPairs = new List<string>();

                foreach (string columnName in columnNames)
                {
                    parameterPairs.Add(string.Format("{0} = @{0}{1}", columnName, seedParameter));
                }

                queryBuilder.AppendLine(string.Format("SET {0}", string.Join(", ", parameterPairs)));
                DynamicParameters parameters = GetEntityParameters(entity, false, seedParameter, ignoreColumns);

                query.Query = queryBuilder;
                query.Parameters = parameters;

                query = Where<TEntity>(query, e => e.Id.Equals(entity.Id), seedParameter);
                return query;
            }

            return new ParsedQuery();
        }

        public override ParsedQuery Delete<TEntity>(TIdType id)
        {
            return Delete<TEntity>(id, string.Empty);
        }

        public override ParsedQuery Delete<TEntity>(TIdType id, string seedParameter)
        {
            if (id != null)
            {
                ParsedQuery query = Where<TEntity>(null, e => e.Id.Equals(id), seedParameter);
                string tableName = GetEntityTableName<TEntity>();
                query.Query.Insert(0, $"DELETE FROM {tableName} ");

                return query;
            }

            return new ParsedQuery();
        }

        public override ParsedQuery Where<TEntity>(ParsedQuery query, Expression<Func<TEntity, bool>> filter)
        {
            return Where(query, filter, string.Empty);
        }

        public override ParsedQuery Where<TEntity>(ParsedQuery query, Expression<Func<TEntity, bool>> filter, string parameterSeed)
        {
            return ProcessFilter(WhereFilterKeyword, query, filter, parameterSeed);
        }

        public override ParsedQuery And<TEntity>(ParsedQuery query, Expression<Func<TEntity, bool>> filter)
        {
            return And(query, filter, string.Empty);
        }

        public override ParsedQuery And<TEntity>(ParsedQuery query, Expression<Func<TEntity, bool>> filter, string parameterSeed)
        {
            return ProcessFilter(AndFilterKeyword, query, filter, parameterSeed);
        }

        public override ParsedQuery Or<TEntity>(ParsedQuery query, Expression<Func<TEntity, bool>> filter)
        {
            return Or(query, filter, string.Empty);
        }

        public override ParsedQuery Or<TEntity>(ParsedQuery query, Expression<Func<TEntity, bool>> filter, string parameterSeed)
        {
            return ProcessFilter(OrFilterKeyword, query, filter, parameterSeed);
        }

        protected virtual ParsedQuery ProcessFilter<TEntity>(string filterKeyword, ParsedQuery query,
            Expression<Func<TEntity, bool>> filter, string parameterSeed)
            where TEntity : IEntity<TIdType>
        {
            if (query == null)
            {
                query = new ParsedQuery();
            }

            query.Query.Append($"{filterKeyword} ");

            TranslatedQuery translatedQuery = _translator.Translate(filter);
            string columnName = GetColumnNameFromPropertyName<TEntity>(translatedQuery.ParameterName);
            if (!translatedQuery.IsNullCheck)
            {
                query.Query.AppendLine(string.Format(translatedQuery.FilterFormat, columnName,
                    $"@{columnName}{parameterSeed}"));

                query.Parameters.Add($"@{columnName}{parameterSeed}", translatedQuery.FilterValue);
            }
            else
            {
                query.Query.AppendLine(string.Format(translatedQuery.FilterFormat, columnName));
            }

            return query;
        }
    }
}
