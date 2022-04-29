using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class AuditWorkScope1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "auditfacilities_name",
                table: "AUDIT_WORK_SCOPE",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "auditprocess_name",
                table: "AUDIT_WORK_SCOPE",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "bussinessactivities_name",
                table: "AUDIT_WORK_SCOPE",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "auditfacilities_name",
                table: "AUDIT_WORK_SCOPE");

            migrationBuilder.DropColumn(
                name: "auditprocess_name",
                table: "AUDIT_WORK_SCOPE");

            migrationBuilder.DropColumn(
                name: "bussinessactivities_name",
                table: "AUDIT_WORK_SCOPE");
        }
    }
}
