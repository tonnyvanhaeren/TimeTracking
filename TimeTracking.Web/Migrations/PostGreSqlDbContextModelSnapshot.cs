using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using TimeTracking.DataAccess;

namespace TimeTracking.Web.Migrations
{
    [DbContext(typeof(PostGreSqlDbContext))]
    partial class PostGreSqlDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rc2-20901");

            modelBuilder.Entity("TimeTracking.General.Models.AppUser", b =>
                {
                    b.Property<string>("Subject");

                    b.Property<DateTime>("BirthDate");

                    b.Property<DateTime>("DateCreated");

                    b.Property<string>("Email");

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("Enabled");

                    b.Property<string>("FamilyName");

                    b.Property<int>("Gender");

                    b.Property<string>("GivenName");

                    b.Property<string>("Password");

                    b.Property<string>("Provider");

                    b.Property<string>("ProviderId");

                    b.Property<string>("SecurityStamp");

                    b.Property<DateTime>("UpdatedTimestamp");

                    b.Property<string>("Username");

                    b.HasKey("Subject");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("Subject");

                    b.ToTable("AppUser");
                });

            modelBuilder.Entity("TimeTracking.General.Models.AppUserPolicy", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("Subject");

                    b.Property<string>("Type");

                    b.Property<DateTime>("UpdatedTimestamp");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("Subject");

                    b.ToTable("AppUserPolicy");
                });

            modelBuilder.Entity("TimeTracking.General.Models.AppUserPolicy", b =>
                {
                    b.HasOne("TimeTracking.General.Models.AppUser")
                        .WithMany()
                        .HasForeignKey("Subject");
                });
        }
    }
}
