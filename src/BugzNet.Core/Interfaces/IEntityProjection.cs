using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BugzNet.Core.Interfaces
{
    public interface IEntityProjection<TEntity>
    {
       
    }

    public static class IEntityProjectionExtensions
    {
        public static Dictionary<string, string> ToDictionary<TEntity>(this IEntityProjection<TEntity> projection) 
        {
            var dictionary = new Dictionary<string, string>();

            var properties = projection.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
            properties.ForEach(p => dictionary.Add(p.Name, (string)p.GetValue(projection)));

            return dictionary;
        }
    }
}
