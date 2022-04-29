using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class updateauditwork2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "auditplan_id",
                table: "AUDIT_WORK",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_WORK_auditplan_id",
                table: "AUDIT_WORK",
                column: "auditplan_id");

            migrationBuilder.AddForeignKey(
                name: "FK_AUDIT_WORK_AUDIT_PLAN_auditplan_id",
                table: "AUDIT_WORK",
                column: "auditplan_id",
                principalTable: "AUDIT_PLAN",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AUDIT_WORK_AUDIT_PLAN_auditplan_id",
                table: "AUDIT_WORK");

            migrationBuilder.DropIndex(
                name: "IX_AUDIT_WORK_auditplan_id",
                table: "AUDIT_WORK");

            migrationBuilder.DropColumn(
                name: "auditplan_id",
                table: "AUDIT_WORK");
        }
    }
}
