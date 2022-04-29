using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class ChangeAuditPlanWork : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "audit_scope",
                table: "AUDIT_WORK_PLAN",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "other_information",
                table: "AUDIT_PLAN",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "audit_scope",
                table: "AUDIT_WORK_PLAN");

            migrationBuilder.DropColumn(
                name: "other_information",
                table: "AUDIT_PLAN");
        }
    }
}
