using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class UpdateAuditwork1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "auditwork_id",
                table: "AUDIT_ASSIGNMENT",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_ASSIGNMENT_auditwork_id",
                table: "AUDIT_ASSIGNMENT",
                column: "auditwork_id");

            migrationBuilder.AddForeignKey(
                name: "FK_AUDIT_ASSIGNMENT_AUDIT_WORK_auditwork_id",
                table: "AUDIT_ASSIGNMENT",
                column: "auditwork_id",
                principalTable: "AUDIT_WORK",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AUDIT_ASSIGNMENT_AUDIT_WORK_auditwork_id",
                table: "AUDIT_ASSIGNMENT");

            migrationBuilder.DropIndex(
                name: "IX_AUDIT_ASSIGNMENT_auditwork_id",
                table: "AUDIT_ASSIGNMENT");

            migrationBuilder.DropColumn(
                name: "auditwork_id",
                table: "AUDIT_ASSIGNMENT");
        }
    }
}
