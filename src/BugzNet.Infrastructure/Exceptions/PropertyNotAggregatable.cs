using BugzNet.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BugzNet.Infrastructure.Exceptions
{
    public class PropertyNotAggregatable : DomainException
    {
        public PropertyNotAggregatable(Type src, PropertyInfo property)
            : base(property.Name, src.Name)
        { }

        public PropertyNotAggregatable(string typeName, string propertyName)
            : base($"Property {propertyName} of {typeName} is non aggregatable.")
        { }
    }
}
