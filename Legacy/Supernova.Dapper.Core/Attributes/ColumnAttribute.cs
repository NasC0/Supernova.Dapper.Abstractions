using System;

namespace Supernova.Dapper.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : BaseAttribute
    {
        public ColumnAttribute(string name)
            : base(name)
        {
        }
    }
}
