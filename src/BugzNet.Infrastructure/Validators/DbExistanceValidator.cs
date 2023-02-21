
using BugzNet.Core.SharedKernel;
using BugzNet.Infrastructure.DataEF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace BugzNet.Infrastructure.Validators
{
    public interface IDbExistanceValidator
    {
        bool MustExistInDb<T>(int id) where T : BaseEntity<int>;
        bool MustExistInDb<T>(Func<T, bool> expression) where T : BaseEntity<int>;
    }

    public class DbExistanceValidator : IDbExistanceValidator
    {
        private readonly BugzNetDataContext _db;
        public DbExistanceValidator(BugzNetDataContext db)
        {
            _db = db;
        }
        public bool MustExistInDb<T>(int id) where T : BaseEntity<int>
        {
            T ent = _db.Set<T>().AsNoTracking().FirstOrDefault(x => x.Id == id);
            return ent != null;
        }

        public bool MustExistInDb<T>(Func<T, bool> expression) where T : BaseEntity<int>
        {
            T ent = _db.Set<T>().AsNoTracking().FirstOrDefault(expression);
            return ent != null;
        }
    }
}
