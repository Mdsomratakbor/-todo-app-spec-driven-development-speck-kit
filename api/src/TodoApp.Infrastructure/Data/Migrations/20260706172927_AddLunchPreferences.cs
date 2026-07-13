using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoApp.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLunchPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LunchPreferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DietaryRestrictions = table.Column<List<string>>(type: "text[]", nullable: true),
                    LunchStartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    LunchEndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    BreakDurationMinutes = table.Column<int>(type: "integer", nullable: true),
                    NotificationsEnabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    FavoriteMeals = table.Column<List<string>>(type: "text[]", nullable: true),
                    ExcludedItems = table.Column<List<string>>(type: "text[]", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LunchPreferences", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LunchPreferences_UserId",
                table: "LunchPreferences",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LunchPreferences");
        }
    }
}
