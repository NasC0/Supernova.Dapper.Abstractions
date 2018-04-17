namespace Supernova.Dapper.Parser.Core.Enums
{
    public enum ColumnTypes
    {
        /// <summary>
        /// Represents all columns in the table. Translates to "*" in SQL.
        /// </summary>
        All,
        /// <summary>
        /// Represents the columns defined in the entity model.
        /// </summary>
        EntityColumns
    }
}
