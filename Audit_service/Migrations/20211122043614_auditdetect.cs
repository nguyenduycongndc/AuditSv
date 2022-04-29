using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class auditdetect : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "control_assessment_id",
                table: "AUDIT_DETECT",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "control_assessment_id1",
                table: "AUDIT_DETECT",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_DETECT_control_assessment_id1",
                table: "AUDIT_DETECT",
                column: "control_assessment_id1");

            migrationBuilder.AddForeignKey(
                name: "FK_AUDIT_DETECT_CONTROL_ASSESSMENT_control_assessment_id1",
                table: "AUDIT_DETECT",
                column: "control_assessment_id1",
                principalTable: "CONTROL_ASSESSMENT",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AUDIT_DETECT_CONTROL_ASSESSMENT_control_assessment_id1",
                table: "AUDIT_DETECT");

            migrationBuilder.DropIndex(
                name: "IX_AUDIT_DETECT_control_assessment_id1",
                table: "AUDIT_DETECT");

            migrationBuilder.DropColumn(
                name: "control_assessment_id",
                table: "AUDIT_DETECT");

            migrationBuilder.DropColumn(
                name: "control_assessment_id1",
                table: "AUDIT_DETECT");
        }
    }
}
