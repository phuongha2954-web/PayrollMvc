using Microsoft.EntityFrameworkCore;
using PayrollMvc.Models;

namespace PayrollMvc.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<PayrollPeriod> PayrollPeriods { get; set; }
        public DbSet<PayrollEntry> PayrollEntries { get; set; }
        public DbSet<AppUser> Users { get; set; }
    }
}
