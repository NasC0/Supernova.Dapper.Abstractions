using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Text;
using Supernova.Dapper.Parser.Core;
using Supernova.Dapper.Parser.Core.Models;

namespace Supernova.Dapper.Parser.SqlServer
{
    public class SqlServerTranslator : ExpressionVisitor, IQueryTranslator
    {
        private const string InitialFormat = " {0} ";

        private StringBuilder _currentFormat;

        private object _filterValue;

        private bool _isNullCheck;

        private string _parameterName;
    

        public TranslatedQuery Translate(Expression expression)
        {
            _currentFormat = new StringBuilder();
            _currentFormat.Append(InitialFormat);

            Visit(expression);

            TranslatedQuery query = new TranslatedQuery
            {
                FilterFormat = _currentFormat.ToString(),
                ParameterName = _parameterName,
                FilterValue = _filterValue,
                IsNullCheck = _isNullCheck
            };

            return query;
        }

        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = ((UnaryExpression)e).Operand;
            }

            return e;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.Name == "Equals")
            {
                _parameterName = ((MemberExpression) m.Object).Member.Name;
                _currentFormat.Append("=");
                VisitExpressionList(m.Arguments);
                return m;
            }

            throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
        }

        protected virtual ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            List<Expression> list = null;
            
            for (int i = 0, n = original.Count; i < n; i++)
            {
                Expression p = this.Visit(original[i]);
                if (list != null)
                {
                    list.Add(p);
                }

                else if (p != original[i])
                {
                    list = new List<Expression>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(p);
                }
            }

            if (list != null)
            {
                return list.AsReadOnly();
            }

            return original;
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    _currentFormat.Append(" NOT ");
                    Visit(u.Operand);
                    break;
                default:
                    throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", u.NodeType));
            }
            return u;
        }
        protected override Expression VisitBinary(BinaryExpression b)
        {
            Visit(b.Left);
            switch (b.NodeType)
            {
                case ExpressionType.And:
                    _currentFormat.Append(" AND ");
                    break;
                case ExpressionType.Or:
                    _currentFormat.Append(" OR");
                    break;
                case ExpressionType.Equal:
                    _currentFormat.Append(" = ");
                    break;
                case ExpressionType.NotEqual:
                    _currentFormat.Append(" <> ");
                    break;
                case ExpressionType.LessThan:
                    _currentFormat.Append(" < ");
                    break;
                case ExpressionType.LessThanOrEqual:
                    _currentFormat.Append(" <= ");
                    break;
                case ExpressionType.GreaterThan:
                    _currentFormat.Append(" > ");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    _currentFormat.Append(" >= ");
                    break;
                default:
                    throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));
            }

            Visit(b.Right);
            return b;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            if (c.Value == null)
            {
                _currentFormat.Append("NULL");
                _isNullCheck = true;
            }
            else
            {
                switch (Type.GetTypeCode(c.Value.GetType()))
                {
                    case TypeCode.Boolean:
                        _currentFormat.Append(((bool)c.Value) ? 1 : 0);
                        break;
                    case TypeCode.String:
                        _currentFormat.Append(" {1}");
                        _filterValue = c.Value;
                        break;
                    case TypeCode.Object:
                        base.VisitConstant(c);
                        //throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", c.Value));
                        break;
                    default:
                        _currentFormat.Append(c.Value);
                        break;
                }
            }

            return c;
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            if (m != null && m.NodeType == ExpressionType.MemberAccess)
            {
                try
                {
                    object value = GetMemberExpressionValue(m);
                    _filterValue = value;
                    _currentFormat.Append(" {1}");
                    return m;
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            if (m?.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
            {
                _parameterName = m.Member.Name;
                return m;
            }

            throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));
        }

        protected virtual object GetMemberExpressionValue(MemberExpression member)
        {
            UnaryExpression objectMember = Expression.Convert(member, typeof(object));
            Expression<Func<object>> getterLambda = Expression.Lambda<Func<object>>(objectMember);
            Func<object> getter = getterLambda.Compile();
            object value = getter.Invoke();

            return value;
        }
    }
}
