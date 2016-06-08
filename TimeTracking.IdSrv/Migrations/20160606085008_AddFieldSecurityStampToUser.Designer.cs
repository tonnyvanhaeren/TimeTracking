using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using TimeTracking.DataAccess;

namespace TimeTracking.IdSrv.Migrations
{
    [DbContext(typeof(PostGreSqlDbContext))]
    [Migration("20160606085008_AddFieldSecurityStampToUser")]
    partial class AddFieldSecurityStampToUser
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rc2-20901");

            modelBuilder.Entity("TimeTracking.General.Models.User", b =>
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

                    b.ToTable("User");
                });

            modelBuilder.Entity("TimeTracking.General.Models.UserClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimIssuer");

                    b.Property<string>("ClaimOriginalIssuer");

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("Subject");

                    b.Property<DateTime>("UpdatedTimestamp");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("Subject");

                    b.ToTable("UserClaim");
                });

            modelBuilder.Entity("TimeTracking.General.Models.UserClaim", b =>
                {
                    b.HasOne("TimeTracking.General.Models.User")
                        .WithMany()
                        .HasForeignKey("Subject");
                });
        }
    }
}
