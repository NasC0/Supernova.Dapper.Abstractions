using System;

namespace Supernova.Dapper.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class TableNameAttribute : BaseAttribute
    {
        public TableNameAttribute(string name)
            : base(name)
        {
        }
    }
}
