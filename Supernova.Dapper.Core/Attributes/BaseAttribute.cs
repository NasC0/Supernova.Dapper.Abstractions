using System;
using System.Collections.Generic;
using System.Text;

namespace Supernova.Dapper.Core.Attributes
{
    public abstract class BaseAttribute : Attribute
    {
        /// <summary>
        /// The name of the table column in the Database.
        /// Optional - use if property name is different from the column name.
        /// </summary>
        public string Name { get; set; }

        protected BaseAttribute(string name)
        {
            Name = name;
        }

        protected BaseAttribute()
        {
        }
    }
}