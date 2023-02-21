
using BugzNet.Core.Extensions;
using BugzNet.Core.SharedKernel;
using BugzNet.Infrastructure.DataEF;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BugzNet.Infrastructure.Extensions
{
    public static class DbContextExtensions
    {
        public static IQueryable<TEntity> GetQueryableFrom<TEntity>(this BugzNetDataContext db, string pkCsv = null)
              where TEntity : BaseEntity<int>
        {
            var entityType = typeof(TEntity);

            var dbSetMethod = _dbSetMethod.MakeGenericMethod(entityType);

            var dbSet = dbSetMethod.Invoke(db, new object[] { });

            var asQueryableMethod = _asQueryableMethod.MakeGenericMethod(entityType);

            var queryable = (IQueryable<TEntity>)asQueryableMethod.Invoke(null, new object[] { dbSet });

            if (string.IsNullOrEmpty(pkCsv)) // TODO: properly display this case on ui
                throw new Exception("No Entities Selected");

            // assumes entities have a single int primary key
            IEnumerable<int> pk = pkCsv.Split(';').Select(p => JsonConvert.DeserializeObject<int[]>(p)).Select(k => (int)k[0]);

            return queryable.Where(e => pk.Contains(e.Id));
        }

        public static async Task<BaseEntity<int>> GetEntityByTypeAndId(this BugzNetDataContext db, Type type, int id)
        {
            var dbSetMethod = _dbSetMethod.MakeGenericMethod(type);

            var dbSet = dbSetMethod.Invoke(db, null) as IQueryable<BaseEntity<int>>;

            return await dbSet.Where(e => e.Id == id).AsNoTracking().FirstOrDefaultAsync();
        }

        public static object GetEntityFromJsonMatch(this BugzNetDataContext db, Type type, string json)
        {
            if (json == null)
                return null;

            var dbSetMethod = _dbSetMethod.MakeGenericMethod(type);

            var queryableDbSet = dbSetMethod.Invoke(db, null);

            var whereMethod = _whereMethod.MakeGenericMethod(type);

            var jsonFields = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            var arg = Expression.Parameter(type, "e");
            Expression expr = null;
            foreach (var field in jsonFields)
            {
                var property = type.GetProperty(field.Key);
                if (!property.PropertyType.IsPrimitive)
                    continue;

                var lval = Expression.PropertyOrField(arg, field.Key);
                if (Nullable.GetUnderlyingType(property.PropertyType) != null)
                    lval = Expression.PropertyOrField(lval, "Value");
                var rval = (Expression) Expression.Constant(field.Value);
                if (rval.Type.IsInteger())
                    rval = Expression.Convert(rval, typeof(int));
                if (expr == null)
                    expr = Expression.Equal(lval, rval);
                else
                    expr = Expression.AndAlso(expr, Expression.Equal(lval, rval));
            }
            var lambda = Expression.Lambda(expr, arg);

            var filteredQueryable = whereMethod.Invoke(null, new object[] { queryableDbSet, lambda });

            _asNoTrackingMethod = _asNoTrackingMethod.MakeGenericMethod(type);
            filteredQueryable = _asNoTrackingMethod.Invoke(null, new object[] { filteredQueryable });

            var firstOrDefaultMethod = _firstOrDefaultMethod.MakeGenericMethod(type);

            return firstOrDefaultMethod.Invoke(null, new object[] { filteredQueryable });
        }

        public static object GetPrincipalEndOfReletionship<PKType>(this BugzNetDataContext db, Type principalType, Type dependentType, PKType dependentId)
        {
            try
            {
                var dbSetMethod = _dbSetMethod.MakeGenericMethod(principalType);

                var queryableDbSet = dbSetMethod.Invoke(db, null);

                var whereMethod = _whereMethod.MakeGenericMethod(principalType);

                var arg = Expression.Parameter(principalType, "e");
                var lval = Expression.PropertyOrField(arg, dependentType.Name + "Id");
                if (Nullable.GetUnderlyingType(principalType.GetProperty(dependentType.Name + "Id").PropertyType) != null)
                    lval = Expression.PropertyOrField(lval, "Value");
                var rval = Expression.Constant(dependentId);
                var expr = Expression.Equal(lval, rval);
                var lambda = Expression.Lambda(expr, arg);

                var filteredQueryable = whereMethod.Invoke(null, new object[] { queryableDbSet, lambda });

                _asNoTrackingMethod = _asNoTrackingMethod.MakeGenericMethod(principalType);
                filteredQueryable = _asNoTrackingMethod.Invoke(null, new object[] { filteredQueryable });

                var firstOrDefaultMethod = _firstOrDefaultMethod.MakeGenericMethod(principalType);

                return firstOrDefaultMethod.Invoke(null, new object[] { filteredQueryable });
            }
            catch
            {
                return null;
            }
        }

        private static MethodInfo _asNoTrackingMethod = typeof(EntityFrameworkQueryableExtensions).GetMethods().Where(m => m.Name == nameof(EntityFrameworkQueryableExtensions.AsNoTracking)).First();
        private static MethodInfo _dbSetMethod = typeof(BugzNetDataContext).GetMethod(nameof(BugzNetDataContext.Set), BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), null);
        private static MethodInfo _whereMethod = typeof(Queryable).GetMethods().Where(m => m.Name == nameof(Queryable.Where)).First();
        private static MethodInfo _firstOrDefaultMethod = typeof(Queryable).GetMethods().Where(m => m.Name == nameof(Queryable.FirstOrDefault)).First();
        private static MethodInfo _asQueryableMethod = typeof(Queryable).GetMethods().Where(m => m.Name == nameof(Queryable.AsQueryable)).First();
        private static IEnumerable<Type> _concreteTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => !t.IsAbstract);
    }
}
