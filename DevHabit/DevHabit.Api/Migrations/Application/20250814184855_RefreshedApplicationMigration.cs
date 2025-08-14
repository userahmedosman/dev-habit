using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevHabit.Api.Migrations.Application;

/// <inheritdoc />
public partial class RefreshedApplicationMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "dev-habit");

        migrationBuilder.CreateTable(
            name: "IdentityUser",
            schema: "dev-habit",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                NormalizedUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                NormalizedEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                AccessFailedCount = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_IdentityUser", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Tags",
            schema: "dev-habit",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Tags", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Users",
            schema: "dev-habit",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Email = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                IdentityId = table.Column<string>(type: "nvarchar(450)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "RefreshToken",
            schema: "dev-habit",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                Token = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                ExpiresAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RefreshToken", x => x.Id);
                table.ForeignKey(
                    name: "FK_RefreshToken_IdentityUser_UserId",
                    column: x => x.UserId,
                    principalSchema: "dev-habit",
                    principalTable: "IdentityUser",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Habits",
            schema: "dev-habit",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                UserId = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                Type = table.Column<int>(type: "int", nullable: false),
                Frequency_Type = table.Column<int>(type: "int", nullable: false),
                Frequency_TimesPerPeriod = table.Column<int>(type: "int", nullable: false),
                Target_Value = table.Column<int>(type: "int", nullable: false),
                Target_Unit = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Status = table.Column<int>(type: "int", nullable: false),
                IsArchived = table.Column<bool>(type: "bit", nullable: false),
                EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                MileStone_Target = table.Column<int>(type: "int", nullable: true),
                MileStone_Current = table.Column<int>(type: "int", nullable: true),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                LastCompletedUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Habits", x => x.Id);
                table.ForeignKey(
                    name: "FK_Habits_Users_UserId",
                    column: x => x.UserId,
                    principalSchema: "dev-habit",
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "HabitTags",
            schema: "dev-habit",
            columns: table => new
            {
                HabitId = table.Column<string>(type: "nvarchar(500)", nullable: false),
                TagId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_HabitTags", x => new { x.HabitId, x.TagId });
                table.ForeignKey(
                    name: "FK_HabitTags_Habits_HabitId",
                    column: x => x.HabitId,
                    principalSchema: "dev-habit",
                    principalTable: "Habits",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_HabitTags_Tags_TagId",
                    column: x => x.TagId,
                    principalSchema: "dev-habit",
                    principalTable: "Tags",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Habits_UserId",
            schema: "dev-habit",
            table: "Habits",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_HabitTags_TagId",
            schema: "dev-habit",
            table: "HabitTags",
            column: "TagId");

        migrationBuilder.CreateIndex(
            name: "IX_RefreshToken_Token",
            schema: "dev-habit",
            table: "RefreshToken",
            column: "Token",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_RefreshToken_UserId",
            schema: "dev-habit",
            table: "RefreshToken",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_Users_Email",
            schema: "dev-habit",
            table: "Users",
            column: "Email",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Users_IdentityId",
            schema: "dev-habit",
            table: "Users",
            column: "IdentityId",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "HabitTags",
            schema: "dev-habit");

        migrationBuilder.DropTable(
            name: "RefreshToken",
            schema: "dev-habit");

        migrationBuilder.DropTable(
            name: "Habits",
            schema: "dev-habit");

        migrationBuilder.DropTable(
            name: "Tags",
            schema: "dev-habit");

        migrationBuilder.DropTable(
            name: "IdentityUser",
            schema: "dev-habit");

        migrationBuilder.DropTable(
            name: "Users",
            schema: "dev-habit");
    }
}
