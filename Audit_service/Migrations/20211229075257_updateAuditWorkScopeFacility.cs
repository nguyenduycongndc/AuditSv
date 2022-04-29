using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class updateAuditWorkScopeFacility : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "brief_review",
                table: "AUDIT_WORK_SCOPE_FACILITY",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "file_type",
                table: "AUDIT_WORK_SCOPE_FACILITY",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "path",
                table: "AUDIT_WORK_SCOPE_FACILITY",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "brief_review",
                table: "AUDIT_WORK_SCOPE_FACILITY");

            migrationBuilder.DropColumn(
                name: "file_type",
                table: "AUDIT_WORK_SCOPE_FACILITY");

            migrationBuilder.DropColumn(
                name: "path",
                table: "AUDIT_WORK_SCOPE_FACILITY");
        }
    }
}
