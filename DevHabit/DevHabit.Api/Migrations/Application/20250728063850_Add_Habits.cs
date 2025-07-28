﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevHabit.Api.Migrations.Application;

/// <inheritdoc />
public partial class Add_Habits : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "dev-habit");

        migrationBuilder.CreateTable(
            name: "habits",
            schema: "dev-habit",
            columns: table => new
            {
                id = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                type = table.Column<int>(type: "int", nullable: false),
                frequency_type = table.Column<int>(type: "int", nullable: false),
                frequency_times_per_period = table.Column<int>(type: "int", nullable: false),
                target_value = table.Column<int>(type: "int", nullable: false),
                target_unit = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                status = table.Column<int>(type: "int", nullable: false),
                is_archived = table.Column<bool>(type: "bit", nullable: false),
                end_date = table.Column<DateOnly>(type: "date", nullable: true),
                mile_stone_target = table.Column<int>(type: "int", nullable: true),
                mile_stone_current = table.Column<int>(type: "int", nullable: true),
                created_at_utc = table.Column<DateTime>(type: "datetime2", nullable: false),
                updated_at_utc = table.Column<DateTime>(type: "datetime2", nullable: true),
                last_completed_utc = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_habits", x => x.id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "habits",
            schema: "dev-habit");
    }
}
