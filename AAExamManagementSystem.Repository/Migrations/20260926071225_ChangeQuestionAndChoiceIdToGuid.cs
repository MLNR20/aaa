using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AAExamManagementSystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class ChangeQuestionAndChoiceIdToGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // SQL Server cannot ALTER an IDENTITY column in place, and int -> uniqueidentifier
            // has no implicit conversion, so the affected columns are recreated instead. New
            // Guid columns are populated first and joined back on the old int ids so existing
            // Question/Choice links are preserved rather than dropped.
            migrationBuilder.AddColumn<Guid>(
                name: "NewId",
                table: "Questions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()");

            migrationBuilder.AddColumn<Guid>(
                name: "NewId",
                table: "Choices",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()");

            migrationBuilder.AddColumn<Guid>(
                name: "NewQuestionId",
                table: "QuestionAndChoices",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "NewChoiceId",
                table: "QuestionAndChoices",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "NewQuestionId",
                table: "Answers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE qc SET qc.NewQuestionId = q.NewId
                FROM QuestionAndChoices qc
                JOIN Questions q ON qc.QuestionId = q.Id;");

            migrationBuilder.Sql(@"
                UPDATE qc SET qc.NewChoiceId = c.NewId
                FROM QuestionAndChoices qc
                JOIN Choices c ON qc.ChoiceId = c.Id;");

            migrationBuilder.Sql(@"
                UPDATE a SET a.NewQuestionId = q.NewId
                FROM Answers a
                JOIN Questions q ON a.QuestionId = q.Id;");

            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Questions_QuestionId",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionAndChoices_Choices_ChoiceId",
                table: "QuestionAndChoices");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionAndChoices_Questions_QuestionId",
                table: "QuestionAndChoices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Questions",
                table: "Questions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Choices",
                table: "Choices");

            migrationBuilder.DropIndex(
                name: "IX_QuestionAndChoices_ChoiceId",
                table: "QuestionAndChoices");

            migrationBuilder.DropIndex(
                name: "IX_QuestionAndChoices_QuestionId",
                table: "QuestionAndChoices");

            migrationBuilder.DropIndex(
                name: "IX_Answers_QuestionId",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Choices");

            migrationBuilder.DropColumn(
                name: "QuestionId",
                table: "QuestionAndChoices");

            migrationBuilder.DropColumn(
                name: "ChoiceId",
                table: "QuestionAndChoices");

            migrationBuilder.DropColumn(
                name: "QuestionId",
                table: "Answers");

            migrationBuilder.RenameColumn(
                name: "NewId",
                table: "Questions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "NewId",
                table: "Choices",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "NewQuestionId",
                table: "QuestionAndChoices",
                newName: "QuestionId");

            migrationBuilder.RenameColumn(
                name: "NewChoiceId",
                table: "QuestionAndChoices",
                newName: "ChoiceId");

            migrationBuilder.RenameColumn(
                name: "NewQuestionId",
                table: "Answers",
                newName: "QuestionId");

            migrationBuilder.AlterColumn<Guid>(
                name: "QuestionId",
                table: "QuestionAndChoices",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ChoiceId",
                table: "QuestionAndChoices",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "QuestionId",
                table: "Answers",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Questions",
                table: "Questions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Choices",
                table: "Choices",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAndChoices_ChoiceId",
                table: "QuestionAndChoices",
                column: "ChoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAndChoices_QuestionId",
                table: "QuestionAndChoices",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_QuestionId",
                table: "Answers",
                column: "QuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Questions_QuestionId",
                table: "Answers",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionAndChoices_Choices_ChoiceId",
                table: "QuestionAndChoices",
                column: "ChoiceId",
                principalTable: "Choices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionAndChoices_Questions_QuestionId",
                table: "QuestionAndChoices",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // The reverse direction re-generates int identity ids; existing Guid-based
            // relationships cannot be mapped back, so links are re-keyed on the new int ids.
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Questions_QuestionId",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionAndChoices_Choices_ChoiceId",
                table: "QuestionAndChoices");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionAndChoices_Questions_QuestionId",
                table: "QuestionAndChoices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Questions",
                table: "Questions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Choices",
                table: "Choices");

            migrationBuilder.DropIndex(
                name: "IX_QuestionAndChoices_ChoiceId",
                table: "QuestionAndChoices");

            migrationBuilder.DropIndex(
                name: "IX_QuestionAndChoices_QuestionId",
                table: "QuestionAndChoices");

            migrationBuilder.DropIndex(
                name: "IX_Answers_QuestionId",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Choices");

            migrationBuilder.DropColumn(
                name: "QuestionId",
                table: "QuestionAndChoices");

            migrationBuilder.DropColumn(
                name: "ChoiceId",
                table: "QuestionAndChoices");

            migrationBuilder.DropColumn(
                name: "QuestionId",
                table: "Answers");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Questions",
                type: "int",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Choices",
                type: "int",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "QuestionId",
                table: "QuestionAndChoices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ChoiceId",
                table: "QuestionAndChoices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QuestionId",
                table: "Answers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Questions",
                table: "Questions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Choices",
                table: "Choices",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAndChoices_ChoiceId",
                table: "QuestionAndChoices",
                column: "ChoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAndChoices_QuestionId",
                table: "QuestionAndChoices",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_QuestionId",
                table: "Answers",
                column: "QuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Questions_QuestionId",
                table: "Answers",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionAndChoices_Choices_ChoiceId",
                table: "QuestionAndChoices",
                column: "ChoiceId",
                principalTable: "Choices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionAndChoices_Questions_QuestionId",
                table: "QuestionAndChoices",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
