namespace Supernova.Dapper.Parser.Core.Models
{
    public class TranslatedQuery
    {
        public string ParameterName { get; set; }

        public string FilterFormat { get; set; }

        public bool IsNullCheck { get; set; }

        public object FilterValue { get; set; }
    }
}
