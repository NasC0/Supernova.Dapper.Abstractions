using System;
using System.Collections.Generic;
using System.Text;

namespace Supernova.Dapper.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKeyAttribute : BaseAttribute
    {
        public PrimaryKeyAttribute()
        {
        }

        public PrimaryKeyAttribute(string name)
            : base(name)
        {
        }
    }
}