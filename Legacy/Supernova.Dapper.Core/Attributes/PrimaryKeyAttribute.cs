using System;

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