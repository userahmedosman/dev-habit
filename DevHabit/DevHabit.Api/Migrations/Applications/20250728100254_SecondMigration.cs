using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevHabit.Api.Migrations.Applications;

/// <inheritdoc />
public partial class SecondMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropPrimaryKey(
            name: "pk_habits",
            schema: "dev-habit",
            table: "habits");

        migrationBuilder.RenameTable(
            name: "habits",
            schema: "dev-habit",
            newName: "Habits",
            newSchema: "dev-habit");

        migrationBuilder.RenameColumn(
            name: "type",
            schema: "dev-habit",
            table: "Habits",
            newName: "Type");

        migrationBuilder.RenameColumn(
            name: "target_value",
            schema: "dev-habit",
            table: "Habits",
            newName: "Target_Value");

        migrationBuilder.RenameColumn(
            name: "target_unit",
            schema: "dev-habit",
            table: "Habits",
            newName: "Target_Unit");

        migrationBuilder.RenameColumn(
            name: "status",
            schema: "dev-habit",
            table: "Habits",
            newName: "Status");

        migrationBuilder.RenameColumn(
            name: "name",
            schema: "dev-habit",
            table: "Habits",
            newName: "Name");

        migrationBuilder.RenameColumn(
            name: "frequency_type",
            schema: "dev-habit",
            table: "Habits",
            newName: "Frequency_Type");

        migrationBuilder.RenameColumn(
            name: "description",
            schema: "dev-habit",
            table: "Habits",
            newName: "Description");

        migrationBuilder.RenameColumn(
            name: "id",
            schema: "dev-habit",
            table: "Habits",
            newName: "Id");

        migrationBuilder.RenameColumn(
            name: "updated_at_utc",
            schema: "dev-habit",
            table: "Habits",
            newName: "UpdatedAtUtc");

        migrationBuilder.RenameColumn(
            name: "last_completed_utc",
            schema: "dev-habit",
            table: "Habits",
            newName: "LastCompletedUtc");

        migrationBuilder.RenameColumn(
            name: "is_archived",
            schema: "dev-habit",
            table: "Habits",
            newName: "IsArchived");

        migrationBuilder.RenameColumn(
            name: "end_date",
            schema: "dev-habit",
            table: "Habits",
            newName: "EndDate");

        migrationBuilder.RenameColumn(
            name: "created_at_utc",
            schema: "dev-habit",
            table: "Habits",
            newName: "CreatedAtUtc");

        migrationBuilder.RenameColumn(
            name: "mile_stone_target",
            schema: "dev-habit",
            table: "Habits",
            newName: "MileStone_Target");

        migrationBuilder.RenameColumn(
            name: "mile_stone_current",
            schema: "dev-habit",
            table: "Habits",
            newName: "MileStone_Current");

        migrationBuilder.RenameColumn(
            name: "frequency_times_per_period",
            schema: "dev-habit",
            table: "Habits",
            newName: "Frequency_TimesPerPeriod");

        migrationBuilder.AddPrimaryKey(
            name: "PK_Habits",
            schema: "dev-habit",
            table: "Habits",
            column: "Id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropPrimaryKey(
            name: "PK_Habits",
            schema: "dev-habit",
            table: "Habits");

        migrationBuilder.RenameTable(
            name: "Habits",
            schema: "dev-habit",
            newName: "habits",
            newSchema: "dev-habit");

        migrationBuilder.RenameColumn(
            name: "Type",
            schema: "dev-habit",
            table: "habits",
            newName: "type");

        migrationBuilder.RenameColumn(
            name: "Target_Value",
            schema: "dev-habit",
            table: "habits",
            newName: "target_value");

        migrationBuilder.RenameColumn(
            name: "Target_Unit",
            schema: "dev-habit",
            table: "habits",
            newName: "target_unit");

        migrationBuilder.RenameColumn(
            name: "Status",
            schema: "dev-habit",
            table: "habits",
            newName: "status");

        migrationBuilder.RenameColumn(
            name: "Name",
            schema: "dev-habit",
            table: "habits",
            newName: "name");

        migrationBuilder.RenameColumn(
            name: "Frequency_Type",
            schema: "dev-habit",
            table: "habits",
            newName: "frequency_type");

        migrationBuilder.RenameColumn(
            name: "Description",
            schema: "dev-habit",
            table: "habits",
            newName: "description");

        migrationBuilder.RenameColumn(
            name: "Id",
            schema: "dev-habit",
            table: "habits",
            newName: "id");

        migrationBuilder.RenameColumn(
            name: "UpdatedAtUtc",
            schema: "dev-habit",
            table: "habits",
            newName: "updated_at_utc");

        migrationBuilder.RenameColumn(
            name: "LastCompletedUtc",
            schema: "dev-habit",
            table: "habits",
            newName: "last_completed_utc");

        migrationBuilder.RenameColumn(
            name: "IsArchived",
            schema: "dev-habit",
            table: "habits",
            newName: "is_archived");

        migrationBuilder.RenameColumn(
            name: "EndDate",
            schema: "dev-habit",
            table: "habits",
            newName: "end_date");

        migrationBuilder.RenameColumn(
            name: "CreatedAtUtc",
            schema: "dev-habit",
            table: "habits",
            newName: "created_at_utc");

        migrationBuilder.RenameColumn(
            name: "MileStone_Target",
            schema: "dev-habit",
            table: "habits",
            newName: "mile_stone_target");

        migrationBuilder.RenameColumn(
            name: "MileStone_Current",
            schema: "dev-habit",
            table: "habits",
            newName: "mile_stone_current");

        migrationBuilder.RenameColumn(
            name: "Frequency_TimesPerPeriod",
            schema: "dev-habit",
            table: "habits",
            newName: "frequency_times_per_period");

        migrationBuilder.AddPrimaryKey(
            name: "pk_habits",
            schema: "dev-habit",
            table: "habits",
            column: "id");
    }
}
