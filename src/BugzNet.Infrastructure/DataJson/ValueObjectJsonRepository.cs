using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using BugzNet.Core.SharedKernel;
using Newtonsoft.Json.Linq;
using BugzNet.Infrastructure.DataJson.Interfaces;

namespace BugzNet.Infrastructure.DataJson
{
    /// <summary>
    /// Generic repository to Retrieve data from json files
    /// </summary>
    /// <typeparam name="T">The typed object to return</typeparam>
    public class ValueObjectJsonRepository<T> 
        : IValueObjectRepository<T> where  T : ValueObject
    {
        private string _sourceFile;
        protected IEnumerable<T> _valueObjects;
        public string JsonDataPath = $"{AppContext.BaseDirectory}/App_Data";

        public ValueObjectJsonRepository(string jsonFile)
        {
            _sourceFile = Path.Combine(JsonDataPath, jsonFile);

            if (!File.Exists(_sourceFile))
                throw new IOException(nameof(jsonFile));

            string jsonValues = File.ReadAllText(_sourceFile);

            var parsedObject = JArray.Parse(jsonValues);

            // get JSON result objects into a list
            IList<JToken> results = parsedObject.Children().ToList();

            // serialize JSON results into .NET objects
            IList<T> _list = new List<T>();
            foreach (JToken result in results)
            {
                // JToken.ToObject is a helper method that uses JsonSerializer internally
                T vobj = result.ToObject<T>();
                _list.Add(vobj);
            }

            _valueObjects = _list;
        }

        public virtual T Find(Func<T, bool> predicate)
        {
            return _valueObjects.First();
        }

        public List<T> List()
        {
            return _valueObjects.ToList();
        }
    }
}
