using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AAExamManagementSystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddDateTrackingToDepartmentsAndCourses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "Departments",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            // Existing departments have never been edited, so they were last updated when created.
            migrationBuilder.Sql("UPDATE Departments SET DateUpdated = CreatedAt");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "Courses",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "Courses",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "Courses");
        }
    }
}
