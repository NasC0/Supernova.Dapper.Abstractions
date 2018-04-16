using System;
using System.Collections.Generic;
using System.Text;

namespace Supernova.Dapper.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : BaseAttribute
    {
        public ColumnAttribute()
        {
        }

        public ColumnAttribute(string name)
            : base(name)
        {
        }
    }
}
