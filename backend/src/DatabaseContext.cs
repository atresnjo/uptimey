using Microsoft.EntityFrameworkCore;
using uptimey.Entities;

namespace uptimey
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<SiteReport> SiteReports { get; set; }

        public DbSet<UserSite> UserSites { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserSite>().HasMany(c => c.Reports).WithOne(c => c.UserSite)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}