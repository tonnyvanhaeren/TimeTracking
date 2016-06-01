using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using TimeTracking.General.Models;

namespace TimeTracking.DataAccess
{
    public class PostGreSqlDbContext : DbContext
    {
        public PostGreSqlDbContext(DbContextOptions<PostGreSqlDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

 
        public DbSet<UserClaim> UserClaims { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>().HasKey(u => u.Subject);
            builder.Entity<User>().HasIndex(u => u.Subject);

            //Set unique index on mail field
            builder.Entity<User>().HasIndex(u => u.Email).IsUnique();

            builder.Entity<UserClaim>().HasKey(r => r.Id);
            builder.Entity<UserClaim>().HasIndex(r => r.Id);
            builder.Entity<UserClaim>().HasIndex(c => c.Subject); //

            // shadow properties
            builder.Entity<User>().Property<DateTime>("UpdatedTimestamp");
            builder.Entity<UserClaim>().Property<DateTime>("UpdatedTimestamp");


            base.OnModelCreating(builder);
        }

        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();

            updateUpdatedProperty<User>();
            updateUpdatedProperty<UserClaim>();

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
