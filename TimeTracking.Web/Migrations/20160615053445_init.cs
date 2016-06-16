using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TimeTracking.Web.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppUser",
                columns: table => new
                {
                    Subject = table.Column<string>(nullable: false),
                    BirthDate = table.Column<DateTime>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false),
                    FamilyName = table.Column<string>(nullable: true),
                    Gender = table.Column<int>(nullable: false),
                    GivenName = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    Provider = table.Column<string>(nullable: true),
                    ProviderId = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    UpdatedTimestamp = table.Column<DateTime>(nullable: false),
                    Username = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUser", x => x.Subject);
                });

            migrationBuilder.CreateTable(
                name: "AppUserPolicy",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:Serial", true),
                    Name = table.Column<string>(nullable: true),
                    Subject = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    UpdatedTimestamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserPolicy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppUserPolicy_AppUser_Subject",
                        column: x => x.Subject,
                        principalTable: "AppUser",
                        principalColumn: "Subject",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppUser_Email",
                table: "AppUser",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppUser_Subject",
                table: "AppUser",
                column: "Subject");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserPolicy_Id",
                table: "AppUserPolicy",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserPolicy_Subject",
                table: "AppUserPolicy",
                column: "Subject");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppUserPolicy");

            migrationBuilder.DropTable(
                name: "AppUser");
        }
    }
}
