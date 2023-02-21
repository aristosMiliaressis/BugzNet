using System;

namespace BugzNet.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class IsAggregatable : Attribute
    {
    }
}
