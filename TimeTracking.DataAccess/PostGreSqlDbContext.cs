using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TimeTracking.General.Models;

namespace TimeTracking.DataAccess
{
    public class PostGreSqlDbContext : DbContext
    {
        public PostGreSqlDbContext(DbContextOptions<PostGreSqlDbContext> options) : base(options)
        {
        }

        public DbSet<AppUser> AppUsers { get; set; }
 
        public DbSet<AppUserPolicy> AppUserPolicies { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<AppUser>().HasKey(u => u.Subject);
            builder.Entity<AppUser>().HasIndex(u => u.Subject);
            builder.Entity<AppUser>().HasIndex(u => u.Email).IsUnique(); //Set unique index on mail field

            builder.Entity<AppUserPolicy>().HasKey(p => p.Id);
            builder.Entity<AppUserPolicy>().HasIndex(p => p.Id);
            builder.Entity<AppUserPolicy>().HasIndex(p => p.Subject);

            // shadow properties
            builder.Entity<AppUser>().Property<DateTime>("UpdatedTimestamp");
            builder.Entity<AppUserPolicy>().Property<DateTime>("UpdatedTimestamp");

            base.OnModelCreating(builder);
        }

        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();

            updateUpdatedProperty<AppUser>();
            updateUpdatedProperty<AppUserPolicy>();

            return base.SaveChanges();
        }

        private void updateUpdatedProperty<T>() where T : class
        {
            var modifiedSourceInfo =
                ChangeTracker.Entries<T>()
                    .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in modifiedSourceInfo)
            {
                entry.Property("UpdatedTimestamp").CurrentValue = DateTime.UtcNow;
            }
        }
    }
}
