using Microsoft.Extensions.Caching.Memory;
using BugzNet.Core.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using BugzNet.Infrastructure.DataJson.Interfaces;

namespace BugzNet.Infrastructure.DataJson
{
    public class BugRepository : ValueObjectJsonRepository<Bug>
    {
        public BugRepository()
            : base("bugs.json") { }

        public override Bug Find(Func<Bug, bool> predicate)
        {
            return _valueObjects.Where(predicate).FirstOrDefault();
        }
    }
}
