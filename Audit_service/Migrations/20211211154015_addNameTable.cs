using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class addNameTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AUDIT_ASSESSMENT_FILE_CONTROL_ASSESSMENT_control_assessment~",
                table: "AUDIT_ASSESSMENT_FILE");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AUDIT_ASSESSMENT_FILE",
                table: "AUDIT_ASSESSMENT_FILE");

            migrationBuilder.RenameTable(
                name: "AUDIT_ASSESSMENT_FILE",
                newName: "CONTROL_ASSESSMENT_FILE");

            migrationBuilder.RenameIndex(
                name: "IX_AUDIT_ASSESSMENT_FILE_control_assessment_id",
                table: "CONTROL_ASSESSMENT_FILE",
                newName: "IX_CONTROL_ASSESSMENT_FILE_control_assessment_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CONTROL_ASSESSMENT_FILE",
                table: "CONTROL_ASSESSMENT_FILE",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_CONTROL_ASSESSMENT_FILE_CONTROL_ASSESSMENT_control_assessme~",
                table: "CONTROL_ASSESSMENT_FILE",
                column: "control_assessment_id",
                principalTable: "CONTROL_ASSESSMENT",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CONTROL_ASSESSMENT_FILE_CONTROL_ASSESSMENT_control_assessme~",
                table: "CONTROL_ASSESSMENT_FILE");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CONTROL_ASSESSMENT_FILE",
                table: "CONTROL_ASSESSMENT_FILE");

            migrationBuilder.RenameTable(
                name: "CONTROL_ASSESSMENT_FILE",
                newName: "AUDIT_ASSESSMENT_FILE");

            migrationBuilder.RenameIndex(
                name: "IX_CONTROL_ASSESSMENT_FILE_control_assessment_id",
                table: "AUDIT_ASSESSMENT_FILE",
                newName: "IX_AUDIT_ASSESSMENT_FILE_control_assessment_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AUDIT_ASSESSMENT_FILE",
                table: "AUDIT_ASSESSMENT_FILE",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_AUDIT_ASSESSMENT_FILE_CONTROL_ASSESSMENT_control_assessment~",
                table: "AUDIT_ASSESSMENT_FILE",
                column: "control_assessment_id",
                principalTable: "CONTROL_ASSESSMENT",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
