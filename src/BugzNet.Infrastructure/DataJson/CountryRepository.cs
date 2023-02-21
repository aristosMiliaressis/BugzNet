using Microsoft.Extensions.Caching.Memory;
using BugzNet.Core.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using BugzNet.Infrastructure.DataJson.Interfaces;

namespace BugzNet.Infrastructure.DataJson
{
    public class CachedCountryRepository : IValueObjectRepository<Country>
    {
        private readonly IValueObjectRepository<Country> _countryRepo;
        private readonly IMemoryCache _memoryCache;

        public CachedCountryRepository(IValueObjectRepository<Country> countryRepo, IMemoryCache memoryCache)
        {
            _countryRepo = countryRepo;
            _memoryCache = memoryCache;
        }

        public Country Find(Func<Country, bool> predicate)
        {
            return List().Where(predicate).FirstOrDefault();
        }

        public List<Country> List()
        {
            if (!_memoryCache.TryGetValue("countries", out List<Country> countries))
            {
                countries = _countryRepo.List();

                var cacheEntryOptions = new MemoryCacheEntryOptions();

                _memoryCache.Set("countries", countries, cacheEntryOptions);
            }

            return countries;
        }
    }
    public class CountryRepository : ValueObjectJsonRepository<Country>
    {
        public CountryRepository()
            : base("countries.json") { }
        public override Country Find(Func<Country, bool> predicate)
        {
            return _valueObjects.Where(predicate).FirstOrDefault();
        }
    }
}
