﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevHabit.Api.Migrations.Application;

/// <inheritdoc />
public partial class Added_GitHub_API_Integration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "GitHubAccessTokens",
            schema: "dev-habit",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                UserId = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                Token = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                ExpiresAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GitHubAccessTokens", x => x.Id);
                table.ForeignKey(
                    name: "FK_GitHubAccessTokens_Users_UserId",
                    column: x => x.UserId,
                    principalSchema: "dev-habit",
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_GitHubAccessTokens_UserId",
            schema: "dev-habit",
            table: "GitHubAccessTokens",
            column: "UserId",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "GitHubAccessTokens",
            schema: "dev-habit");
    }
}
