using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class ReportAuditWork2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "rating_level_total",
                table: "REPORT_AUDIT_WORK",
                newName: "audit_rating_level_total");

            migrationBuilder.RenameColumn(
                name: "audit_rating_report",
                table: "AUDIT_WORK_SCOPE",
                newName: "audit_rating_level_report");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "audit_rating_level_total",
                table: "REPORT_AUDIT_WORK",
                newName: "rating_level_total");

            migrationBuilder.RenameColumn(
                name: "audit_rating_level_report",
                table: "AUDIT_WORK_SCOPE",
                newName: "audit_rating_report");
        }
    }
}
