using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AAExamManagementSystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicantUserLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Applicants",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_UserId",
                table: "Applicants",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Applicants_AspNetUsers_UserId",
                table: "Applicants",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applicants_AspNetUsers_UserId",
                table: "Applicants");

            migrationBuilder.DropIndex(
                name: "IX_Applicants_UserId",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Applicants");
        }
    }
}
