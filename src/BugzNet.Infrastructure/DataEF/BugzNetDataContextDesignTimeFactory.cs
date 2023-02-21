using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace BugzNet.Infrastructure.DataEF
{
    /// <summary>
    /// This interface is picked up by ef tools
    /// </summary>
    internal class BugzNetDataContextDesignTimeFactory : IDesignTimeDbContextFactory<BugzNetDataContext>
    {
        BugzNetDataContext IDesignTimeDbContextFactory<BugzNetDataContext>.CreateDbContext(string[] args)
        {
            string connStr = "Server=.;Database=BugzNet;User id=sa;password=12345678;MultipleActiveResultSets=true";

            return new BugzNetDataContext(new DbContextOptionsBuilder().UseSqlServer(connStr).Options);
        }
    }
}