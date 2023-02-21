using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BugzNet.Core.ValueObjects;

namespace BugzNet.Infrastructure.DataJson
{
    public class CurrencyRepository : ValueObjectJsonRepository<Currency>
    {
        public CurrencyRepository()
            : base("currencies.json") { }
        public override Currency Find(Func<Currency, bool> predicate)
        {
            return _valueObjects.Where(predicate).FirstOrDefault();
        }
    }
}
