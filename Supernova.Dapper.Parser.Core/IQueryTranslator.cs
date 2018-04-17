using System.Linq.Expressions;
using Supernova.Dapper.Parser.Core.Models;

namespace Supernova.Dapper.Parser.Core
{
    public interface IQueryTranslator
    {
        TranslatedQuery Translate(Expression expression);
    }
}
