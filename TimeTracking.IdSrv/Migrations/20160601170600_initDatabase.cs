using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TimeTracking.IdSrv.Migrations
{
    public partial class initDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Subject = table.Column<string>(nullable: false),
                    BirthDate = table.Column<DateTime>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false),
                    FamilyName = table.Column<string>(nullable: true),
                    Gender = table.Column<int>(nullable: false),
                    GivenName = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    Provider = table.Column<string>(nullable: true),
                    ProviderId = table.Column<string>(nullable: true),
                    UpdatedTimestamp = table.Column<DateTime>(nullable: false),
                    Username = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Subject);
                });

            migrationBuilder.CreateTable(
                name: "UserClaim",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:Serial", true),
                    ClaimIssuer = table.Column<string>(nullable: true),
                    ClaimOriginalIssuer = table.Column<string>(nullable: true),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    Subject = table.Column<string>(nullable: true),
                    UpdatedTimestamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaim_User_Subject",
                        column: x => x.Subject,
                        principalTable: "User",
                        principalColumn: "Subject",
                        onDelete: ReferentialAction.Restrict); //todo: cascade delete
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "User",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Subject",
                table: "User",
                column: "Subject");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaim_Id",
                table: "UserClaim",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaim_Subject",
                table: "UserClaim",
                column: "Subject");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserClaim");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
