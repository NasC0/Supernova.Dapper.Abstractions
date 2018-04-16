using System;
using System.Collections.Generic;
using System.Text;

namespace Supernova.Dapper.Core.Attributes
{
    public class TableNameAttribute : BaseAttribute
    {
        public TableNameAttribute(string name)
            : base(name)
        {
        }
    }
}
