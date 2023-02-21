using System;
using System.Collections.Generic;
using BugzNet.Core.SharedKernel;

namespace BugzNet.Infrastructure.DataJson.Interfaces
{
    public interface IValueObjectRepository<T> where T : ValueObject
    {
        T Find(Func<T, bool> predicate);
        List<T> List();
    }
}
