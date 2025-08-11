﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevHabit.Api.Migrations.HabitTags;

/// <inheritdoc />
public partial class AddedUserEntity : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
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
            name: "Users",
            schema: "dev-habit");
    }
}
