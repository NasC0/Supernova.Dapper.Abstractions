using System.Text;
using Dapper;

namespace Supernova.Dapper.Parser.Core.Models
{
    public class ParsedQuery
    {
        public ParsedQuery()
        {
            Parameters = new DynamicParameters();
            Query = new StringBuilder();
        }

        public DynamicParameters Parameters { get; set; }

        public StringBuilder Query { get; set; }
    }
}
